import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { PRIORITY_OPTIONS, Task } from '../../models/task.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
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
  styleUrls: ['./task-form.component.scss'],
})
export class TaskFormComponent implements OnChanges {
  @Input() selectedTask: Task | null = null;
  @Input() isLoading = false;

  @Output() formSubmit = new EventEmitter<{
    title: string;
    description?: string;
    isCompleted: boolean;
    priority: 0 | 1 | 2;
    dueDate?: string;
  }>();
  @Output() cancelEdit = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);

  readonly priorityOptions = PRIORITY_OPTIONS;

  form = this.fb.nonNullable.group({
    title:       ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    isCompleted: [false],
    priority:    [1 as 0 | 1 | 2],
    dueDate:     [null as Date | null],
  });

  get isEditMode(): boolean {
    return this.selectedTask !== null;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isLoading']) {
      this.isLoading ? this.form.disable({ emitEvent: false }) : this.form.enable({ emitEvent: false });
    }

    if (changes['selectedTask']) {
      if (this.selectedTask) {
        this.form.patchValue({
          title:       this.selectedTask.title,
          description: this.selectedTask.description ?? '',
          isCompleted: this.selectedTask.isCompleted,
          priority:    this.selectedTask.priority,
          dueDate:     this.selectedTask.dueDate ? new Date(this.selectedTask.dueDate) : null,
        });
      } else {
        this.resetForm();
      }
    }
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
      dueDate:     raw.dueDate ? (raw.dueDate as Date).toISOString() : undefined,
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
