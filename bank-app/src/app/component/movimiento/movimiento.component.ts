import { MovimientoService } from '../../services/movimiento.service';
import { CuentaService } from '../../services/cuenta.service';
import { Movimiento } from '../../models/movimiento.model';
import { Cuenta } from '../../models/cuenta.model';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-movimiento',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule, FormsModule],
  templateUrl: './movimiento.component.html',
})
export class MovimientoComponent implements OnInit {
  form: FormGroup;
  movimientoId?: string;
  movimientos: Movimiento[] = [];
  movimientosFiltrados: Movimiento[] = [];
  cuentas: Cuenta[] = [];
  filtro: string = '';
  paginaActual: number = 1;
  itemsPorPagina: number = 10;
  totalPaginas: number = 1;
  cuentaSeleccionada: string = '';
  mensaje: string = '';
  tipoMensaje: 'exito' | 'error' | '' = '';
  mostrarMensaje: boolean = false;
  movimientoAEliminar: Movimiento | null = null;

  constructor(
    private fb: FormBuilder,
    private movimientoService: MovimientoService,
    private cuentaService: CuentaService,
    public route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      tipoMovimiento: ['', Validators.required],
      valor: [0, [Validators.required, Validators.min(0.01)]],
      cuentaId: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    const path = this.route.snapshot.routeConfig?.path;
    this.movimientoId = this.route.snapshot.paramMap.get('id') || undefined;

    if (path?.includes('crear')) {
      this.cargarCuentas();
    } else if (path?.includes('editar') && this.movimientoId) {
      this.cargarCuentasYMovimiento(this.movimientoId);
    } else {
      this.cargarCuentasYMovimientos();
    }
  }

  cargarCuentas(): void {
    this.cuentaService.getAll().subscribe({
      next: cuentas => this.cuentas = cuentas,
      error: err => alert('Error al cargar cuentas: ' + err.message)
    });
  }

  cargarCuentasYMovimientos(): void {
    forkJoin({
      cuentas: this.cuentaService.getAll(),
      movimientos: this.movimientoService.getAll()
    }).subscribe({
      next: ({ cuentas, movimientos }) => {
        this.cuentas = cuentas;
        this.movimientos = movimientos.map(m => ({
          ...m,
          cuenta: cuentas.find(c => c.cuentaId === m.cuentaId)
        }));
        this.totalPaginas = Math.ceil(this.movimientos.length / this.itemsPorPagina);
        this.actualizarMovimientosFiltrados(this.movimientos);
      },
      error: err => alert('Error al cargar datos: ' + err.message)
    });
  }

  cargarCuentasYMovimiento(id: string): void {
    forkJoin({
      cuentas: this.cuentaService.getAll(),
      movimientos: this.movimientoService.getAll()
    }).subscribe({
      next: ({ cuentas, movimientos }) => {
        this.cuentas = cuentas;
        const movimiento = movimientos.find(m => m.movimientoId === id);
        if (movimiento) this.form.patchValue(movimiento);
      },
      error: err => alert('Error al cargar movimiento: ' + err.message)
    });
  }

  filtrarMovimientos(): void {
    const texto = this.filtro.toLowerCase().trim();
    let resultados = this.movimientos;

    if (this.cuentaSeleccionada) {
      resultados = resultados.filter(m => m.cuentaId === this.cuentaSeleccionada);
    }

    resultados = resultados.filter(m =>
      m.tipoMovimiento.toLowerCase().includes(texto) ||
      m.cuenta?.numeroCuenta.toLowerCase().includes(texto) ||
      m.cuenta?.tipoCuenta.toLowerCase().includes(texto)
    );

    this.totalPaginas = Math.ceil(resultados.length / this.itemsPorPagina);
    this.paginaActual = 1;
    this.actualizarMovimientosFiltrados(resultados);
  }

  actualizarMovimientosFiltrados(listaFiltrada: Movimiento[]): void {
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    this.movimientosFiltrados = listaFiltrada.slice(inicio, fin);
  }

  cambiarPagina(nuevaPagina: number): void {
    if (nuevaPagina < 1 || nuevaPagina > this.totalPaginas) return;

    this.paginaActual = nuevaPagina;
    const texto = this.filtro.toLowerCase().trim();
    let resultados = this.movimientos;

    if (this.cuentaSeleccionada) {
      resultados = resultados.filter(m => m.cuentaId === this.cuentaSeleccionada);
    }

    resultados = resultados.filter(m =>
      m.tipoMovimiento.toLowerCase().includes(texto) ||
      m.cuenta?.numeroCuenta.toLowerCase().includes(texto) ||
      m.cuenta?.tipoCuenta.toLowerCase().includes(texto)
    );

    this.actualizarMovimientosFiltrados(resultados);
  }

  guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const movimiento: Movimiento = {
      ...this.form.value,
      fecha: new Date().toISOString()
    };

    const request = this.movimientoId
      ? this.movimientoService.update(this.movimientoId, movimiento)
      : this.movimientoService.create(movimiento.cuentaId, movimiento);

    request.subscribe({
      next: () => {
        this.mostrarAlerta(this.movimientoId ? 'Movimiento actualizado correctamente.' : 'Movimiento registrado con éxito.', 'exito');
        setTimeout(() => {
          this.router.navigate(['/movimientos']);
          this.form.reset({ valor: 0 });
          this.cargarCuentasYMovimientos();
        }, 2000);
      },
      error: err => this.manejarErrores(err)
    });
  }

  confirmarEliminacion(movimiento: Movimiento): void {
    this.movimientoAEliminar = movimiento;
  }

  cancelarEliminacion(): void {
    this.movimientoAEliminar = null;
  }

  eliminarConfirmado(): void {
    if (!this.movimientoAEliminar) return;

    this.movimientoService.delete(this.movimientoAEliminar.movimientoId!).subscribe({
      next: () => {
        this.mostrarAlerta('Movimiento eliminado correctamente', 'exito');
        this.movimientos = this.movimientos.filter(m => m.movimientoId !== this.movimientoAEliminar!.movimientoId);
        this.filtrarMovimientos();
        this.movimientoAEliminar = null;
      },
      error: err => {
        this.manejarErrores(err);
        this.movimientoAEliminar = null;
      }
    });
  }



  mostrarAlerta(mensaje: string, tipo: 'exito' | 'error') {
    this.mensaje = mensaje;
    this.tipoMensaje = tipo;
    this.mostrarMensaje = true;
    setTimeout(() => this.cerrarMensaje(), 4000);
  }

  cerrarMensaje() {
    this.mostrarMensaje = false;
    this.mensaje = '';
    this.tipoMensaje = '';
  }

  manejarErrores(error: any): void {
    const backendError = error?.error;

    if (typeof backendError === 'string') {
      this.mostrarAlerta(backendError, 'error');
      return;
    }

    if (backendError?.error && typeof backendError.error === 'string') {
      this.mostrarAlerta(backendError.error, 'error');
      return;
    }

    if (backendError?.errors) {
      const mensajes: string[] = [];
      for (const campo in backendError.errors) {
        if (backendError.errors.hasOwnProperty(campo)) {
          mensajes.push(`${campo}: ${backendError.errors[campo].join(', ')}`);
        }
      }
      this.mostrarAlerta('Errores de validación: ' + mensajes.join(' | '), 'error');
      return;
    }

    const mensaje = backendError?.message || error.message || 'Error inesperado.';
    this.mostrarAlerta('Error: ' + mensaje, 'error');
  }

}
