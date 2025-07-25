import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReporteService {
  private apiUrl = 'http://localhost:5115/api/Reportes';

  constructor(private http: HttpClient) {}

  obtenerReporte(clienteId: string, desde: string, hasta: string, formato: 'json' | 'pdf' = 'json'): Observable<any> {
    const params = { clienteId, desde, hasta, formato };
    return this.http.get(this.apiUrl, { params });
  }
}
