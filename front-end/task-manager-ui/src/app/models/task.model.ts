export type TaskPriority = 0 | 1 | 2;

export interface Task {
  id: number;
  title: string;
  description?: string;
  isCompleted: boolean;
  priority: TaskPriority;
  priorityLabel: string;
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority: TaskPriority;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  isCompleted: boolean;
  priority: TaskPriority;
  dueDate?: string;
}

export interface TaskFilterParams {
  search?: string;
  isCompleted?: boolean;
  priority?: TaskPriority;
  sortBy?: string;
  sortDir?: 'asc' | 'desc';
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export const PRIORITY_OPTIONS = [
  { value: 0 as TaskPriority, label: 'Low' },
  { value: 1 as TaskPriority, label: 'Medium' },
  { value: 2 as TaskPriority, label: 'High' },
];

export const SORT_OPTIONS = [
  { value: 'createdat', label: 'Date Created' },
  { value: 'title',     label: 'Title' },
  { value: 'priority',  label: 'Priority' },
  { value: 'duedate',   label: 'Due Date' },
];
