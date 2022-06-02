export class NavItems {
  private static NavMenuList = [
    {
      DisplayName: 'DASHBOARD',
      Link: '/dashboard',
      FAIcon: 'fas fa-th-list',
      ZegaOnly: false
    },
    {
      DisplayName: 'STRATEGIES',
      Link: '/strategies',
      FAIcon: 'fas fa-user-tie',
      ZegaOnly: true
    },
    {
      DisplayName: 'ACCOUNTS',
      Link: '/accounts',
      FAIcon: 'fas fa-chart-pie',
      ZegaOnly: false
    },
    {
      DisplayName: 'MODELS',
      Link: '/models',
      FAIcon: 'fas fa-cubes',
      ZegaOnly: false
    },
    {
      DisplayName: 'MODEL SLEEVES',
      Link: '/sleeves',
      FAIcon: 'fas fa-th',
      ZegaOnly: true
    },
    {
      DisplayName: 'USER MANAGEMENT',
      Link: '/users',
      FAIcon: 'fas fa-users-cog',
      ZegaOnly: true
    },
    {
      DisplayName: 'IMPORT PROFILE',
      Link: '/import',
      FAIcon: 'fas fa-file-import',
      ZegaOnly: true
    },
    {
      DisplayName: 'IMPORT HISTORY',
      Link: '/importhistory',
      FAIcon: 'fa fa-history',
      ZegaOnly: true
    },
    {
      DisplayName: 'AUDIT LOG',
      Link: '/audit',
      FAIcon: 'fas fa-clipboard-list',
      ZegaOnly: false
    }
  ];

  static getPermittedModules(IsAdmin: boolean) {
    return IsAdmin ? this.NavMenuList : this.NavMenuList.filter(m => !m.ZegaOnly);
  }

  static checkModulePermissions(link: string, isAdmin: boolean) {
    var module = this.NavMenuList.find(m => m.Link === link);
    return isAdmin ? module != null : module != null && !module.ZegaOnly;
  }
  
  static getBreadcrumbTitle(urlSeg: Array<string>) {
    var link = urlSeg.length ? '/'+ urlSeg[0]: '/';
    var module = this.NavMenuList.find(m => m.Link === link);
    return module != null ? module.DisplayName : '';
  }

}
