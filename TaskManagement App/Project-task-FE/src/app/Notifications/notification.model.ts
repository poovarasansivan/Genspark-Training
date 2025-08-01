export interface Notification {
  id: string;
  message: string;
  title: string;
  type: 'TaskCreated' | 'TaskUpdated' | 'TaskDeleted' | 'TaskAssigned' | 'TaskUnassigned' | 'TaskReopened';
  createdAt: string;
  isRead: boolean;
  UnassignedByName?: string;
  Status?: string;
  PreviousStatus?: string;
  AssignedToName?: string;
  CreatedByName?: string;
  UpdatedByName?: string;
  AssignedToId?: string;
  Description?: string;
}