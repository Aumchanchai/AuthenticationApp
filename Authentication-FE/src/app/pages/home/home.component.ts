import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
// เปลี่ยนเป็นดึง Username จาก Token
  username: string | null = null;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) { }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.authService.getProfile().subscribe({
      next: (response) => {
        this.username = response.username;
      },
      error: (err) => {
        alert(err);
      }
    });
  }

  logout() {
    this.authService.logout();
  }

}
