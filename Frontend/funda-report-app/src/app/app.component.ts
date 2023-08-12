import { Component, OnInit } from '@angular/core';
import { BackendService } from './backend.service';
import { MakelaarReportModel } from './models/MakelaarReportModel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'funda-report-app';
  report: MakelaarReportModel = {};
  failed = false;

  constructor(private _backendService: BackendService) {}

  ngOnInit() {
    this._backendService.getReport().subscribe(report => {
      if (report) {
        this.report = report;
      } else {
        this.failed = true;
      }
    })
  }
}
