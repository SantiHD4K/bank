export interface Cliente {
  id: string;
  nombre: string;
  genero?: string;
  edad: number;
  identificacion: string;
  direccion: string;
  telefono: string;
  contraseña: string;
  estado: boolean;
}
