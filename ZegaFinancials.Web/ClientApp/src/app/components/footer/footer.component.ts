import { Component, OnInit } from '@angular/core';
import { FooterService } from '../../services/footer/footer.service';
import { Constants } from '../../support/constants/constants';
@Component({
  selector: 'zega-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.less']
})
export class FooterComponent implements OnInit {
  version: any;
  constructor(private footerService: FooterService) {
  
  }

  ngOnInit(): void {
    this.getVersionInfo();
  }
  private getVersionInfo() {
    this.footerService.getVersionInfo()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse)
          this.version = (response['response']);
        console.log(this.version);
      });
  }
}
