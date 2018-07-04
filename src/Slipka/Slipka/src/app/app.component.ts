import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http'
import { ProxySettings } from './proxySettings';
import { MongoSettings } from './mongoSettings';
import { StateElement } from './stateElement';
import { Status } from './status';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private _httpService: Http) { }

  proxySettings: ProxySettings = null;
  mongoSettings: MongoSettings = null;
  stateElements: StateElement[] = null;
  status: Status = null;


  ngOnInit() {
    this._httpService.get('/api/status/ProxySettings').subscribe(values => {
      this.proxySettings = values.json() as ProxySettings;
    });

    this._httpService.get('/api/status/MongoSettings').subscribe(values => {
      this.mongoSettings = values.json() as MongoSettings;
    });

    this._httpService.get('/api/status/State').subscribe(values => {
      this.stateElements = values.json() as StateElement[];
    });

    this._httpService.get('/api/status/Status').subscribe(values => {
      this.status = values.json() as Status;
    });
  }
}
