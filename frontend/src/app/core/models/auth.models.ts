export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiraEnUtc: string;
  username: string;
  rol: string;
}

// Lo que se guarda en el navegador de la sesión actual.
export interface Sesion {
  token: string;
  expiraEnUtc: string;
  username: string;
  rol: string;
}
