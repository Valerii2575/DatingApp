import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers(){
    return this.http.get<Member[]>(this.baseUrl + 'users');   //, this.getHttpOptions() 
  }

  getMember(userName: string){
    console.log(userName);
    return this.http.get<Member>(this.baseUrl + 'users/' + userName);  //, this.getHttpOptions()
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member);
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  //This process was added in interceptors
  // getHttpOptions(){
  //   const userString = localStorage.getItem('user');
  //   if(!userString) return;

  //   const user = JSON.parse(userString);
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token 
  //     })
  //   }
  // }
}
