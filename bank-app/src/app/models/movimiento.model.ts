import { Cuenta } from './cuenta.model';

export interface Movimiento {
  movimientoId?: string;
  fecha: string;
  tipoMovimiento: string;
  valor: number;
  saldo?: number;
  cuentaId: string;
  cuenta?: Cuenta;
}
