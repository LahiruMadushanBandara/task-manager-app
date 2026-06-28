import { ChangeDetectionStrategy, Component, computed, effect, inject, input, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PRIORITY_OPTIONS, Task, TaskPriority, UpdateTaskRequest } from '../../models/task.model';

@Component({
    selector: 'app-task-form',
    imports: [
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatCheckboxModule,
        MatIconModule,
        MatDividerModule,
    ],
    templateUrl: './task-form.component.html',
    styleUrls: ['./task-form.component.scss']
})
export class TaskFormComponent {
  readonly selectedTask = input<Task | null>(null);
  readonly isLoading = input(false);

  readonly formSubmit = output<UpdateTaskRequest>();
  readonly cancelEdit = output<void>();

  private readonly fb = inject(FormBuilder);

  readonly priorityOptions = PRIORITY_OPTIONS;

  readonly form = this.fb.nonNullable.group({
    title:       ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    isCompleted: [false],
    priority:    [1 as TaskPriority],
    dueDate:     [null as Date | null],
  });

  readonly isEditMode = computed(() => this.selectedTask() !== null);

  constructor() {
    // Toggle the form's enabled state in lock-step with the parent's submit flag.
    effect(() => {
      this.isLoading() ? this.form.disable({ emitEvent: false }) : this.form.enable({ emitEvent: false });
    });

    // Hydrate the form whenever the selected task changes (edit vs. create).
    effect(() => {
      const task = this.selectedTask();
      if (task) {
        this.form.patchValue({
          title:       task.title,
          description: task.description ?? '',
          isCompleted: task.isCompleted,
          priority:    task.priority,
          dueDate:     task.dueDate ? new Date(task.dueDate) : null,
        });
      } else {
        this.resetForm();
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.formSubmit.emit({
      title:       raw.title.trim(),
      description: raw.description?.trim() || undefined,
      isCompleted: raw.isCompleted,
      priority:    raw.priority,
      dueDate:     raw.dueDate ? raw.dueDate.toISOString() : undefined,
    });
  }

  cancel(): void {
    this.resetForm();
    this.cancelEdit.emit();
  }

  private resetForm(): void {
    this.form.reset({ priority: 1, isCompleted: false });
  }
}
