export interface LoginRequest {
  username: string;
  password: string;
}

export interface StoredCredentials {
  username: string;
  encoded: string; // base64(username:password)
}
