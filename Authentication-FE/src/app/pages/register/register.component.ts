import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required] // เพิ่ม field นี้
    }, { validators: passwordMatchValidator });
  }

  onSubmit(): void {
    this.errorMessage = '';
    // Check both form.valid and form.errors to ensure the passwords match.
    if (this.registerForm.valid) { 
      // When calling the API, we only pass the username and password.
      const userToRegister = {
        username: this.registerForm.value.username,
        password: this.registerForm.value.password
      };

      this.authService.register(userToRegister).subscribe({
        next: () => {
          alert('สมัครสมาชิกสำเร็จ! กรุณาเข้าสู่ระบบ');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.errorMessage = err.error || 'สมัครสมาชิกไม่สำเร็จ กรุณาลองใหม่อีกครั้ง';
          console.error(err);
        }
      });
    } else {
        // In case the passwords do not match or other validations do not pass
        if (this.registerForm.errors?.['mismatch']) {
            this.errorMessage = 'Password และ Confirm Password ไม่ตรงกัน';
        } else {
            this.errorMessage = 'กรุณากรอกข้อมูลให้ครบถ้วนและถูกต้อง';
        }
    }
  }

}

export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): { [key: string]: boolean } | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  
  // Return null if controls are not initialized or password matches
  if (!password || !confirmPassword) {
    return null;
  }
  
  // Return null if passwords match, otherwise return error object
  return password.value === confirmPassword.value ? null : { 'mismatch': true };
};
