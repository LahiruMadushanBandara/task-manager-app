export interface StoredCredentials {
  username: string;
  encoded: string; // base64(username:password)
}
