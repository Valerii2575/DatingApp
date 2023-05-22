import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-catd',
  templateUrl: './member-catd.component.html',
  styleUrls: ['./member-catd.component.css']
})
export class MemberCatdComponent implements OnInit {
  @Input() member: Member | undefined 

  constructor(){}
  ngOnInit(): void {
  }
}
