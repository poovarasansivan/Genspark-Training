export class PlanDetailsModel {
  id!: string;
  planName!: string;
  planDescription!: string;
  startDate!: Date;
  endDate!: Date;
  coachName!: string;
  coachEmail!: string;
  clientName!: string;
  clientEmail!: string;
  isCompleted!: boolean;
}
