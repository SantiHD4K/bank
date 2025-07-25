import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ClienteService } from '../../services/cliente.service';
import { Cliente } from '../../models/cliente.model';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-cliente',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule, FormsModule],
  templateUrl: './cliente.component.html',
})
export class ClienteComponent implements OnInit {
  form: FormGroup;
  clienteId?: string;
  clientes: Cliente[] = [];
  clientesFiltrados: Cliente[] = [];
  filtro: string = '';
  paginaActual: number = 1;
  itemsPorPagina: number = 10;
  totalPaginas: number = 1;
  clienteAEliminar: Cliente | null = null;
  mensaje: string = '';
  tipoMensaje: 'exito' | 'error' | '' = '';
  mostrarMensaje: boolean = false;

  constructor(
    private fb: FormBuilder,
    private clienteService: ClienteService,
    public route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      nombre: ['', Validators.required],
      genero: [''],
      edad: [18, [Validators.required, Validators.min(18)]],
      identificacion: ['', Validators.required],
      direccion: ['', Validators.required],
      telefono: ['', [Validators.required, Validators.pattern(/^\d{10,}$/)]],
      contraseña: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
      estado: [true],
    });
  }

  ngOnInit(): void {
    const path = this.route.snapshot.routeConfig?.path;
    this.clienteId = this.route.snapshot.paramMap.get('id') || undefined;

    if (path?.includes('crear')) {
    } else if (path?.includes('editar') && this.clienteId) {
      this.cargarCliente(this.clienteId);
    } else {
      this.cargarClientes();
    }
  }

  mostrarAlerta(mensaje: string, tipo: 'exito' | 'error') {
    this.mensaje = mensaje;
    this.tipoMensaje = tipo;
    this.mostrarMensaje = true;

    // Ocultar automáticamente después de 4 segundos
    setTimeout(() => this.cerrarMensaje(), 4000);
  }

  cerrarMensaje() {
    this.mostrarMensaje = false;
    this.mensaje = '';
    this.tipoMensaje = '';
  }
  manejarErrores(error: any): void {
    console.error('Error capturado:', error);

    const backendError = error?.error;

    // Caso 1: error = "mensaje" (string plano directo)
    if (typeof backendError === 'string') {
      this.mostrarAlerta(backendError, 'error');
      return;
    }

    // Caso 2: error = { error: "mensaje" }
    if (backendError?.error && typeof backendError.error === 'string') {
      this.mostrarAlerta(backendError.error, 'error');
      return;
    }

    // Caso 3: error = { errors: { campo: ["msg"] } } (ModelState)
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

    // Caso 4: error inesperado
    const mensaje = backendError?.message || error.message || 'Error inesperado.';
    this.mostrarAlerta('Error: ' + mensaje, 'error');
  }


  cargarClientes(): void {
    this.clienteService.getAll().subscribe({
      next: (data) => {
        this.clientes = data;
        this.totalPaginas = Math.ceil(data.length / this.itemsPorPagina);
        this.actualizarClientesFiltrados(data);
      },
      error: (err) => this.mostrarAlerta('Error al cargar clientes: ' + err.message, 'error')
    });
  }

  cargarCliente(id: string): void {
    this.clienteService.getById(id).subscribe({
      next: (cliente) => this.form.patchValue(cliente),
      error: (err) => this.mostrarAlerta('Error al cargar cliente: ' + err.message, 'error')
    });
  }

  guardar(): void {
    if (this.form.valid) {
      const datos = this.form.value;
      if (this.clienteId) datos.id = this.clienteId;

      const request = this.clienteId
        ? this.clienteService.update(this.clienteId, datos)
        : this.clienteService.create(datos);

      request.subscribe({
        next: () => {
          const mensaje = this.clienteId ? 'Cliente actualizado con éxito.' : 'Cliente creado con éxito.';
          this.mostrarAlerta(mensaje, 'exito');

          setTimeout(() => {
            this.router.navigate(['/clientes']);
            this.form.reset({ estado: true });
            this.clienteService.getAll().subscribe(data => this.clientes = data);
          }, 2000);
        },
        error: err => this.manejarErrores(err)
      });

    } else {
      this.form.markAllAsTouched();
    }
  }

  confirmarEliminacion(cliente: Cliente): void {
    this.clienteAEliminar = cliente;
  }

  cancelarEliminacion(): void {
    this.clienteAEliminar = null;
  }

  eliminarConfirmado(): void {
    if (!this.clienteAEliminar) return;

    this.clienteService.delete(this.clienteAEliminar.id).subscribe({
      next: () => {
        this.mostrarAlerta('Cliente eliminado correctamente', 'exito');
        this.clientes = this.clientes.filter(c => c.id !== this.clienteAEliminar!.id);
        this.actualizarClientesFiltrados(this.clientes);
        this.clienteAEliminar = null;
      },
      error: (err) => {
        this.manejarErrores(err);
        this.clienteAEliminar = null;
      }
    });
  }


  filtrarClientes(): void {
    const texto = this.filtro.toLowerCase().trim();
    const resultados = this.clientes.filter(c =>
      c.nombre.toLowerCase().includes(texto) ||
      c.identificacion.toLowerCase().includes(texto)
    );
    this.totalPaginas = Math.ceil(resultados.length / this.itemsPorPagina);
    this.paginaActual = 1;
    this.actualizarClientesFiltrados(resultados);
  }

  actualizarClientesFiltrados(listaFiltrada: Cliente[]): void {
    const inicio = (this.paginaActual - 1) * this.itemsPorPagina;
    const fin = inicio + this.itemsPorPagina;
    this.clientesFiltrados = listaFiltrada.slice(inicio, fin);
  }

  cambiarPagina(nuevaPagina: number): void {
    if (nuevaPagina < 1 || nuevaPagina > this.totalPaginas) return;

    this.paginaActual = nuevaPagina;
    const texto = this.filtro.toLowerCase().trim();
    const resultados = this.clientes.filter(c =>
      c.nombre.toLowerCase().includes(texto) ||
      c.identificacion.toLowerCase().includes(texto)
    );

    this.actualizarClientesFiltrados(resultados);
  }
}
