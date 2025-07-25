import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Cuenta } from '../models/cuenta.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CuentaService {
  private apiUrl = 'http://localhost:5115/api/Cuentas';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Cuenta[]> {
    return this.http.get<Cuenta[]>(this.apiUrl);
  }
  
  getById(id: string): Observable<Cuenta> {
    return this.http.get<Cuenta>(`${this.apiUrl}/${id}`);
  }

  getByCliente(clienteId: string): Observable<Cuenta[]> {
    return this.http.get<Cuenta[]>(`${this.apiUrl}/cliente/${clienteId}`);
  }

  create(cuenta: Cuenta): Observable<Cuenta> {
    return this.http.post<Cuenta>(this.apiUrl, cuenta);
  }

  update(id: string, cuenta: Cuenta): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, cuenta);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  cambiarEstado(id: string, estado: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/estado`, estado);
  }
}
