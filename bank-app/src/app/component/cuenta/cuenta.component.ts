import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CuentaService } from '../../services/cuenta.service';
import { ClienteService } from '../../services/cliente.service';
import { Cuenta } from '../../models/cuenta.model';
import { Cliente } from '../../models/cliente.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { forkJoin, Observable } from 'rxjs';

@Component({
    selector: 'app-cuenta',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        RouterModule
    ],
    templateUrl: './cuenta.component.html',
})
export class CuentaComponent implements OnInit {
    form: FormGroup;
    cuentaId?: string;
    cuentas: Cuenta[] = [];
    cuentasFiltradas: Cuenta[] = [];
    clientes: Cliente[] = [];
    filtro: string = '';
    paginaActual: number = 1;
    itemsPorPagina: number = 10;
    totalPaginas: number = 1;
    mensaje: string = '';
    tipoMensaje: 'exito' | 'error' | '' = '';
    mostrarMensaje: boolean = false;
    cuentaAEliminar: Cuenta | null = null;



    constructor(
        private fb: FormBuilder,
        private cuentaService: CuentaService,
        private clienteService: ClienteService,
        public route: ActivatedRoute,
        private router: Router
    ) {
        this.form = this.fb.group({
            numeroCuenta: [
                '',
                [
                    Validators.required,
                    Validators.minLength(6),
                    Validators.maxLength(12)
                ]
            ],
            tipoCuenta: ['', Validators.required],
            saldoInicial: [0, [Validators.required, Validators.min(0)]],
            clienteId: ['', Validators.required],
            estado: [true]
        });
    }

    ngOnInit(): void {
        const path = this.route.snapshot.routeConfig?.path;
        this.cuentaId = this.route.snapshot.params['id'];

        if (path?.includes('crear')) {
            this.cargarClientes();
        } else if (path?.includes('editar') && this.cuentaId) {
            this.cargarCuenta(this.cuentaId);
        } else {
            this.cargarDatos();
        }
    }

    cargarDatos(): void {
        forkJoin({
            clientes: this.clienteService.getAll(),
            cuentas: this.cuentaService.getAll()
        }).subscribe({
            next: (result: { clientes: Cliente[], cuentas: Cuenta[] }) => {
                this.clientes = result.clientes;
                this.cuentas = result.cuentas.map(cuenta => {
                    const cliente = this.clientes.find(c => c.id === cuenta.clienteId);
                    return { ...cuenta, cliente };
                });
                this.totalPaginas = Math.ceil(this.cuentas.length / this.itemsPorPagina);
                this.actualizarCuentasFiltradas(this.cuentas);
            },
            error: (err: any) => alert('Error al cargar datos: ' + err.message)
        });
    }

    cargarClientes(): void {
        this.clienteService.getAll().subscribe({
            next: (clientes: Cliente[]) => this.clientes = clientes,
            error: (err: any) => alert('Error al cargar clientes: ' + err.message)
        });
    }

    cargarCuenta(id: string): void {
        forkJoin({
            cuenta: this.cuentaService.getById(id),
            clientes: this.clienteService.getAll()
        }).subscribe({
            next: ({ cuenta, clientes }) => {
                this.clientes = clientes;
                this.form.patchValue({
                    numeroCuenta: cuenta.numeroCuenta,
                    tipoCuenta: cuenta.tipoCuenta,
                    saldoInicial: cuenta.saldoInicial,
                    clienteId: cuenta.clienteId,
                    estado: cuenta.estado
                });
            },
            error: (err) => alert('Error al cargar cuenta: ' + err.message)
        });
    }



    filtrarCuentas(): void {
        const texto = this.filtro.toLowerCase().trim();
        const resultados = this.cuentas.filter(c =>
            c.numeroCuenta.toLowerCase().includes(texto) ||
            c.tipoCuenta.toLowerCase().includes(texto) ||
            (c.cliente?.nombre.toLowerCase().includes(texto))
        );

        this.totalPaginas = Math.ceil(resultados.length / this.itemsPorPagina);
        this.paginaActual = 1;
        this.actualizarCuentasFiltradas(resultados);
    }

    actualizarCuentasFiltradas(listaFiltrada: Cuenta[]): void {
        const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
        const fin = inicio + this.itemsPorPagina;
        this.cuentasFiltradas = listaFiltrada.slice(inicio, fin);
    }

    cambiarPagina(nuevaPagina: number): void {
        if (nuevaPagina < 1 || nuevaPagina > this.totalPaginas) return;

        this.paginaActual = nuevaPagina;
        const texto = this.filtro.toLowerCase().trim();
        const resultados = this.cuentas.filter(c =>
            c.numeroCuenta.toLowerCase().includes(texto) ||
            c.tipoCuenta.toLowerCase().includes(texto) ||
            (c.cliente?.nombre.toLowerCase().includes(texto))
        );

        this.actualizarCuentasFiltradas(resultados);
    }

    guardar(): void {
        if (this.form.valid) {
            const datos: Cuenta = {
                cuentaId: this.cuentaId!,
                numeroCuenta: this.form.value.numeroCuenta,
                tipoCuenta: this.form.value.tipoCuenta,
                saldoInicial: this.form.value.saldoInicial,
                clienteId: this.form.value.clienteId,
                estado: this.form.value.estado
            };
            if (this.cuentaId) datos.cuentaId = this.cuentaId;

            const request: Observable<any> = this.cuentaId
                ? this.cuentaService.update(this.cuentaId, datos)
                : this.cuentaService.create(datos);


            request.subscribe({
                next: () => {
                    this.mostrarAlerta(this.cuentaId ? 'Cuenta actualizada correctamente.' : 'Cuenta creada con éxito.', 'exito');
                    setTimeout(() => {
                        this.router.navigate(['/cuentas']);
                        this.form.reset({ estado: true, saldoInicial: 0 });
                        this.cargarDatos();
                    }, 2000);
                },
                error: err => this.manejarErrores(err)
            });
        } else {
            this.form.markAllAsTouched();
        }
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


    confirmarEliminacion(cuenta: Cuenta): void {
        this.cuentaAEliminar = cuenta;
    }

    cancelarEliminacion(): void {
        this.cuentaAEliminar = null;
    }

    eliminarConfirmado(): void {
        if (!this.cuentaAEliminar) return;

        this.cuentaService.delete(this.cuentaAEliminar.cuentaId!).subscribe({
            next: () => {
                this.mostrarAlerta('Cuenta eliminada correctamente', 'exito');
                this.cuentas = this.cuentas.filter(c => c.cuentaId !== this.cuentaAEliminar!.cuentaId);
                this.actualizarCuentasFiltradas(this.cuentas);
                this.cuentaAEliminar = null;
            },
            error: err => {
                this.manejarErrores(err);
                this.cuentaAEliminar = null;
            }
        });
    }

}