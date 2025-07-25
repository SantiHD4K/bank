import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { ClienteComponent } from './component/cliente/cliente.component';
import { CuentaComponent } from './component/cuenta/cuenta.component';
import { MovimientoComponent } from './component/movimiento/movimiento.component';
import { ReporteComponent } from './component/reporte/reporte.component';
export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', redirectTo: 'clientes', pathMatch: 'full' },
      { path: 'clientes', component: ClienteComponent },
      { path: 'clientes', component: ClienteComponent },
      { path: 'clientes/crear', component: ClienteComponent },
      { path: 'clientes/editar/:id', component: ClienteComponent },
      { path: 'cuentas', component: CuentaComponent },
      { path: 'cuentas/crear', component: CuentaComponent },
      { path: 'cuentas/editar/:id', component: CuentaComponent },
      { path: 'movimientos', component: MovimientoComponent },
      { path: 'movimientos/crear', component: MovimientoComponent },
      { path: 'movimientos/editar/:id', component: MovimientoComponent },
      { path: 'reportes', component: ReporteComponent },
    ]
  }

];