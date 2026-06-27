import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { Task, TaskPriority } from '../../models/task.model';

@Component({
    selector: 'app-task-list',
    imports: [
        DatePipe,
        MatCardModule,
        MatCheckboxModule,
        MatIconModule,
        MatButtonModule,
        MatChipsModule,
        MatTooltipModule,
        MatDividerModule,
    ],
    templateUrl: './task-list.component.html',
    styleUrls: ['./task-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskListComponent {
  readonly tasks = input<Task[]>([]);
  readonly selectedTaskId = input<number | null>(null);

  readonly taskSelected = output<Task>();
  readonly toggleComplete = output<Task>();
  readonly deleteTask = output<Task>();

  priorityClass(priority: TaskPriority): string {
    return priority === 2 ? 'priority-high' : priority === 1 ? 'priority-medium' : 'priority-low';
  }

  isOverdue(task: Task): boolean {
    return !task.isCompleted && !!task.dueDate && new Date(task.dueDate) < new Date();
  }
}
