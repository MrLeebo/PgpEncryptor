# PgpEncryptor
Implementation of Bouncy Castle in C# using streams to handle large files without loading everything into memory

## Usage

```cs
PgpEncryptor.Encryptor.EncryptFile("Message.txt", "Encrypted.txt.pgp", "PublicKey.asc");
```
