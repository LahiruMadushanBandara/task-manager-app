import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { EMPTY, merge } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, startWith, switchMap } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog/confirm-dialog.component';
import { TaskListComponent } from '../task-list/task-list.component';
import { TaskFormComponent } from '../task-form/task-form.component';
import { TaskService } from '../task.service';
import { AuthService } from '../../auth/auth.service';
import { SnackbarService } from '../../shared/snackbar.service';
import { PRIORITY_OPTIONS, SORT_OPTIONS, Task, TaskFilterParams } from '../../models/task.model';

@Component({
  selector: 'app-task-board',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatDividerModule,
    TaskListComponent,
    TaskFormComponent,
  ],
  templateUrl: './task-board.component.html',
  styleUrls: ['./task-board.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskBoardComponent {
  private readonly taskService = inject(TaskService);
  private readonly auth = inject(AuthService);
  private readonly snackbar = inject(SnackbarService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);

  readonly tasks = signal<Task[]>([]);
  readonly selectedTask = signal<Task | null>(null);
  readonly isLoadingTasks = signal(false);
  readonly isSubmitting = signal(false);

  readonly priorityOptions = PRIORITY_OPTIONS;
  readonly sortOptions = SORT_OPTIONS;

  filterForm = this.fb.group({
    search:      [''],
    isCompleted: [null as boolean | null],
    priority:    [null as number | null],
    sortBy:      ['createdat'],
    sortDir:     ['desc' as 'asc' | 'desc'],
  });

  constructor() {
    const search$ = this.filterForm.controls.search.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
    );

    merge(
      search$,
      this.filterForm.controls.isCompleted.valueChanges,
      this.filterForm.controls.priority.valueChanges,
      this.filterForm.controls.sortBy.valueChanges,
      this.filterForm.controls.sortDir.valueChanges,
    ).pipe(
      startWith(null),
      takeUntilDestroyed(),
      switchMap(() => {
        this.isLoadingTasks.set(true);
        return this.taskService.getAll(this.buildFilters()).pipe(
          catchError(() => {
            this.isLoadingTasks.set(false);
            this.snackbar.error('Failed to load tasks.');
            return EMPTY;
          })
        );
      }),
    ).subscribe(tasks => {
      this.tasks.set(tasks);
      this.isLoadingTasks.set(false);
    });
  }

  get username(): string {
    return this.auth.getCredentials()?.username ?? '';
  }

  private buildFilters(): TaskFilterParams {
    const raw = this.filterForm.getRawValue();
    return {
      search:      raw.search?.trim() || undefined,
      isCompleted: raw.isCompleted ?? undefined,
      priority:    raw.priority != null ? (raw.priority as 0 | 1 | 2) : undefined,
      sortBy:      raw.sortBy || 'createdat',
      sortDir:     (raw.sortDir as 'asc' | 'desc') || 'desc',
    };
  }

  onTaskSelected(task: Task): void {
    this.selectedTask.set(this.selectedTask()?.id === task.id ? null : task);
  }

  onNewTask(): void {
    this.selectedTask.set(null);
  }

  onFormSubmit(data: {
    title: string;
    description?: string;
    isCompleted: boolean;
    priority: 0 | 1 | 2;
    dueDate?: string;
  }): void {
    this.isSubmitting.set(true);
    const current = this.selectedTask();

    if (current) {
      this.taskService.update(current.id, data).subscribe({
        next: updated => {
          this.tasks.update(list => list.map(t => t.id === updated.id ? updated : t));
          this.selectedTask.set(null);
          this.isSubmitting.set(false);
          this.snackbar.success('Task updated.');
        },
        error: () => {
          this.isSubmitting.set(false);
          this.snackbar.error('Failed to update task.');
        },
      });
    } else {
      this.taskService.create(data).subscribe({
        next: created => {
          this.tasks.update(list => [created, ...list]);
          this.isSubmitting.set(false);
          this.snackbar.success('Task created.');
        },
        error: () => {
          this.isSubmitting.set(false);
          this.snackbar.error('Failed to create task.');
        },
      });
    }
  }

  onToggleComplete(task: Task): void {
    this.taskService.toggleComplete(task.id).subscribe({
      next: updated => {
        this.tasks.update(list => list.map(t => t.id === updated.id ? updated : t));
        if (this.selectedTask()?.id === updated.id) this.selectedTask.set(updated);
      },
      error: () => this.snackbar.error('Failed to update task status.'),
    });
  }

  onDeleteTask(task: Task): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '380px',
      data: {
        title: 'Delete Task',
        message: `Are you sure you want to delete "${task.title}"? This action cannot be undone.`,
        confirmLabel: 'Delete',
      },
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;

      this.taskService.delete(task.id).subscribe({
        next: () => {
          this.tasks.update(list => list.filter(t => t.id !== task.id));
          if (this.selectedTask()?.id === task.id) this.selectedTask.set(null);
          this.snackbar.success('Task deleted.');
        },
        error: () => this.snackbar.error('Failed to delete task.'),
      });
    });
  }

  clearFilters(): void {
    this.filterForm.reset({ sortBy: 'createdat', sortDir: 'desc' });
  }

  logout(): void {
    this.auth.logout();
  }
}
