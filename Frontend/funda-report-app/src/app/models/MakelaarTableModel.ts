import { MakelaarRowModel } from "./MakelaarRowModel";

export interface MakelaarTableModel {
    query?: string;
    numberOfApiRequests?: number;
    totalTimePreparingTable?: number;
    totalTimeWaitingOnRateLimit?: number;
    rows?: MakelaarRowModel[];
}