import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class ZegaRouteGuard implements CanActivate {

  constructor(private authService: AuthenticationService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean  {
    let isAuthorized = this.authService.checkUserModulePermission(route.url[0].path);
    if (!isAuthorized) {
      this.router.navigate(['/login']);
      sessionStorage.clear();
    }
    return isAuthorized;
  }
}
