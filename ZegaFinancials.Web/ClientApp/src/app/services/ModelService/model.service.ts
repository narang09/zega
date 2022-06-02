import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiURLs } from '../../support/apiURLs/api-urls';

@Injectable({
  providedIn: 'root'
})
export class ModelService {

  constructor(private http: HttpClient) { }

  deleteStrategies(strategyIds: Array<number>) {
    return this.http.post(ApiURLs.deleteStrategiesAPI, strategyIds)
      .pipe(response => response);
  }

  saveStrategy(strategy: any) {
    return this.http.post(ApiURLs.saveStrategyAPI, strategy)
      .pipe(response => response);
  }

  getStrategy(strategyId: number) {
    return this.http.post(ApiURLs.getStrategyAPI, strategyId)
      .pipe(response => response);
  }

  deleteSleeeves(sleeveIds: Array<number>) {
    return this.http.post(ApiURLs.deleteSleevesAPI, sleeveIds)
      .pipe(response => response);
  }

  saveSleeve(strategy: any) {
    return this.http.post(ApiURLs.saveSleeveAPI, strategy)
      .pipe(response => response);
  }

  getSleeve(strategyId: number) {
    return this.http.post(ApiURLs.getSleeveAPI, strategyId)
      .pipe(response => response);
  }

  deleteModels(modelIds: Array<number>) {
    return this.http.post(ApiURLs.deleteModelsAPI, modelIds)
      .pipe(response => response);
  }

  saveModel(model: any) {
    return this.http.post(ApiURLs.saveModelAPI, model)
      .pipe(response => response);
  }

  getModel(modelId: number) {
    return this.http.post(ApiURLs.getModelAPI, modelId)
      .pipe(response => response);
  }

  getModelDropdown() {
    return this.http.get(ApiURLs.getModelDropdownsAPI)
      .pipe(response => response);
  }
 

}
