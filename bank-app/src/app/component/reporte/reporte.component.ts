import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReporteService } from '../../services/reporte.service';
import { ClienteService } from '../../services/cliente.service';
import { Cliente } from '../../models/cliente.model';

@Component({
    selector: 'app-reporte',
    standalone: true,
    templateUrl: './reporte.component.html',
    imports: [
        ReactiveFormsModule,
        FormsModule,
        CommonModule,
        RouterModule
    ]
})
export class ReporteComponent implements OnInit {
    form: FormGroup;
    movimientos: any[] = [];
    clientes: Cliente[] = [];
    cuenta: any = null;
    pdfBase64: string = '';

    mensaje: string = '';
    tipoMensaje: 'exito' | 'error' | '' = '';
    mostrarMensaje: boolean = false;

    constructor(
        private fb: FormBuilder,
        private reporteService: ReporteService,
        private clienteService: ClienteService
    ) {
        this.form = this.fb.group({
            clienteId: ['', Validators.required],
            desde: ['', Validators.required],
            hasta: ['', Validators.required],
        });
    }

    ngOnInit(): void {
        this.clienteService.getAll().subscribe({
            next: (data) => this.clientes = data,
            error: () => this.mostrarAlerta('Error al cargar clientes', 'error')
        });
    }

    generarReporte(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            this.mostrarAlerta('Completa todos los campos obligatorios para generar el reporte.', 'error');
            return;
        }

        const { clienteId, desde, hasta } = this.form.value;

        this.reporteService.obtenerReporte(clienteId, desde, hasta, 'json').subscribe({
            next: (data) => {
                if (data && data.length > 0) {
                    const cuenta = data[0];
                    this.cuenta = {
                        numeroCuenta: cuenta.numeroCuenta,
                        tipoCuenta: cuenta.tipoCuenta,
                        saldo: cuenta.saldo,
                        totalCreditos: cuenta.totalCreditos,
                        totalDebitos: cuenta.totalDebitos
                    };
                    this.movimientos = cuenta.movimientos || [];
                    this.mostrarAlerta('Reporte generado con éxito.', 'exito');
                } else {
                    this.cuenta = null;
                    this.movimientos = [];
                    this.mostrarAlerta('No se encontraron movimientos.', 'error');
                }
            },
            error: () => this.mostrarAlerta('Error al generar el reporte.', 'error')
        });

        this.reporteService.obtenerReporte(clienteId, desde, hasta, 'pdf').subscribe({
            next: (res) => {
                console.log('Base64 recibido:', res.pdfBase64?.substring(0, 50));
                this.pdfBase64 = res.pdfBase64;
            },
            error: () => this.mostrarAlerta('No se pudo generar el PDF.', 'error')
        });
    }

    descargarPDF(): void {
        if (!this.pdfBase64) {
            this.mostrarAlerta('No hay PDF disponible para descargar.', 'error');
            return;
        }

        try {
            const pdfURL = `data:application/pdf;base64,${this.pdfBase64}`;
            const a = document.createElement('a');
            a.href = pdfURL;
            a.download = 'reporte.pdf';
            a.click();
        } catch (error) {
            this.mostrarAlerta('Error al intentar descargar el PDF.', 'error');
            console.error('Error al descargar PDF:', error);
        }
    }


    private convertirBase64APdfBlob(base64: string): Blob {
        const binary = window.atob(base64);
        const len = binary.length;
        const bytes = new Uint8Array(len);

        for (let i = 0; i < len; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        return new Blob([bytes], { type: 'application/pdf' });
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
}
