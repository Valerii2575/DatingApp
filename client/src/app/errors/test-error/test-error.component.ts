import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {

    baseUrl = 'https://localhost:7026/api/';

    constructor(private http: HttpClient) {}

    ngOnInit(): void
    {

    }

    get401Error()
    {
        this.http.get(this.baseUrl + 'Buggy/auth').subscribe({
          next: response => console.log(response),
          error: error => console.log(error)
        })
    }

    get404Error()
    {
        this.http.get(this.baseUrl + 'Buggy/not-found').subscribe({
          next: response => console.log(response),
          error: error => console.log(error)
        })
    }

    get501Error()
    {
        this.http.get(this.baseUrl + 'Buggy/server-error').subscribe({
          next: response => console.log(response),
          error: error => console.log(error)
        })
    }

    get400Error()
    {
        this.http.get(this.baseUrl + 'Buggy/bad-request').subscribe({
          next: response => console.log(response),
          error: error => console.log(error)
        })
    }
}
