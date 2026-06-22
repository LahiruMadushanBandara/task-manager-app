import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
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
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TaskListComponent {
  @Input() tasks: Task[] = [];
  @Input() selectedTaskId: number | null = null;

  @Output() taskSelected = new EventEmitter<Task>();
  @Output() toggleComplete = new EventEmitter<Task>();
  @Output() deleteTask = new EventEmitter<Task>();

  priorityColor(priority: number): string {
    return priority === 2 ? 'warn' : priority === 1 ? 'accent' : 'primary';
  }

  priorityClass(priority: number): string {
    return priority === 2 ? 'priority-high' : priority === 1 ? 'priority-medium' : 'priority-low';
  }

  isOverdue(task: Task): boolean {
    return !task.isCompleted && !!task.dueDate && new Date(task.dueDate) < new Date();
  }
}
