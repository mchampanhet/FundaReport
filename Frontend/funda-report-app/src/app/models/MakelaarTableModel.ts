import { MakelaarRowModel } from "./MakelaarRowModel";

export interface MakelaarTableModel {
    Query?: string;
    NumberOfApiRequests?: number;
    TotalTimePreparingTable?: number;
    TotalTimeWaitingOnRateLimit?: number;
    Rows?: MakelaarRowModel[];
}