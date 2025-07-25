import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Movimiento } from '../models/movimiento.model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class MovimientoService {
    private apiUrl = 'http://localhost:5115/api/Movimientos';

    constructor(private http: HttpClient) { }

    getAll(): Observable<Movimiento[]> {
        return this.http.get<Movimiento[]>(this.apiUrl);
    }

    getByCuenta(cuentaId: string): Observable<Movimiento[]> {
        return this.http.get<Movimiento[]>(`${this.apiUrl}/cuenta/${cuentaId}`);
    }

    create(cuentaId: string, movimiento: Movimiento): Observable<any> {
        return this.http.post(`${this.apiUrl}/${cuentaId}`, movimiento);
    }

    update(movimientoId: string, movimiento: Movimiento): Observable<any> {
        return this.http.put(`${this.apiUrl}/${movimientoId}`, movimiento);
    }
    
    delete(movimientoId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${movimientoId}`);
    }
}

