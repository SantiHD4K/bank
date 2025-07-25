import { Cliente } from './cliente.model';

export interface Cuenta {
  cuentaId?: string;
  numeroCuenta: string;
  tipoCuenta: string;
  saldoInicial: number;
  estado?: boolean;
  clienteId: string;
  cliente?: Cliente;
}
