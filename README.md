# PhotoCrypto - Završni rad 3. godine studija

## Pregled aplikacije

PhotoCrypto je WinForms desktop aplikacija (.NET 8) koja demonstrira enkripciju
i dekripciju podataka koristeæi fotografiju kao izvor kriptografskog kljuèa.

---

## Arhitektura – dijagram

```
PhotoCrypto/
+¦¦ Program.cs                          ‹ Ulazna toèka aplikacije
+¦¦ Forms/
-   +¦¦ MainForm.cs                     ‹ Logika GUI-a (event handleri)
-   L¦¦ MainForm.Designer.cs            ‹ Layout kontrola
+¦¦ Crypto/
-   +¦¦ Encryption/
-   -   +¦¦ IEncryptionStrategy.cs      ‹ Strategy suèelje
-   -   L¦¦ AesCbcEncryptionStrategy.cs ‹ AES-256-CBC implementacija
-   L¦¦ KeyDerivation/
-       +¦¦ IKeyDerivationMethod.cs     ‹ Suèelje derivacije
-       L¦¦ HashKeyDerivation.cs        ‹ SHA-256 / SHA-512
L¦¦ Analysis/
    L¦¦ ImageAnalyzer.cs                ‹ Entropija, varijanca, ocjena
```

---

## Dizajn obrasci (Design Patterns)

### Strategy (GoF)
Korišten na **dvije razine**:

| Suèelje | Implementacije |
|---------|----------------|
| `IEncryptionStrategy` | `AesCbcEncryptionStrategy` |
| `IKeyDerivationMethod` | `HashKeyDerivation` |

Dodavanje novog algoritma: implementiraj suèelje › registriraj u `MainForm` listi.
Ostatak koda se **ne mijenja** (OCP – Open/Closed Principle).

---

## Metoda derivacije kljuèa

### Hash derivacija (`HashKeyDerivation`)
```
Fotografija (sve bajte piksela)
        ¡  SHA-256 ili SHA-512
Kljuè (32 ili 64 bajta)
```
- **Prednosti:** deterministièki, brz, avalanche efekt
- **Nedostaci:** osjetljiv na JPEG rekompresiju (mijenja piksele)
- **Preporuèeni format:** PNG ili BMP

---

## Algoritam enkripcije

### AES-256-CBC (`AesCbcEncryptionStrategy`)
```
Kljuè (256 bita) ‹ SHA-256 normalizacija ako je drugaèija duljina
Sluèajni IV (128 bita) ‹ generirano za svaku enkripciju
Format izlaza: [IV (16 B)] ? [Ciphertext]
```
- Standard: NIST FIPS 197 (AES) + SP 800-38A (CBC naèin)
- Padding: PKCS7
- Sigurnost: industrijski standard; siguran uz pravilno rukovanje IV-om

---

## Analiza prikladnosti fotografije (`ImageAnalyzer`)

### Shannonova entropija
```
H = -? p(x) · log2(p(x))
```
Raèuna se na histogramu razina sive (256 razina). Maksimum: **8 bita/px**.

### Varijanca intenziteta
```
?2 = (1/N) · ?(x? - µ)2
```
Mjeri rasprostranjenost svjetline piksela. Maksimum za uniformnu distribuciju: ~16 256.

### Ocjena prikladnosti
```
Ocjena = (Entropija/8) × 60%  +  min(Varijanca/5000, 1) × 40%  ×  100
```

| Ocjena | Kategorija |
|--------|------------|
| ? 70   | ? Odlièno  |
| 50–69  | ?? Dobro   |
| 30–49  | ?? Osrednje|
| < 30   | ? Loše    |

---

## Format razmjene podataka

Izlaz enkripcije prikazan je u **Base64** enkodiranju – standardni format
za prikaz binarnih podataka u tekstualnom obliku (RFC 4648).

Za dekripciju: zalijepiti Base64 niz u polje za unos, odabrati isti
algoritam i metodu derivacije, kliknuti „Dekriptiraj".

---

## Upute za pokretanje

**Preduvjet:** .NET 8 SDK – https://dotnet.microsoft.com/download

```bash
cd PhotoCrypto
dotnet build
dotnet run
```

---

## Proširivanje aplikacije

### Novi algoritam enkripcije
1. Implementiraj `IEncryptionStrategy`
2. Dodaj instancu u listu `_encryptionStrategies` u `MainForm.cs`

### Nova metoda derivacije
1. Implementiraj `IKeyDerivationMethod`
2. Dodaj instancu u listu `_derivationMethods` u `MainForm.cs`

Nema potrebe za izmjenama ostatka koda – Strategy obrazac to garantira.