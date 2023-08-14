import { HttpClient } from "@angular/common/http";
import { MakelaarReportModel } from "./models/MakelaarReportModel";
import { Injectable } from "@angular/core";
import { Observable, catchError, of } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class BackendService {
    constructor(private _httpClient: HttpClient) {}

  getReport(): Observable<MakelaarReportModel | null> {
    return this._httpClient.get<MakelaarReportModel>('https://localhost:7102/Report/GetStandardMakelaarReport')
        .pipe(
            catchError(error => of(null))
        );
  }
}