# 🎵 GrooveOn -- Music Streaming & Recommendation Application

## 📌 Introduction

GrooveOn je full-stack aplikacija za streaming muzike koja omogućava
korisnicima da pretražuju pjesme, artiste i albume, kreiraju playliste i
koriste sistem preporuka.


## 🛠️ Tehnologije i alati

-   Git
-   Docker & Docker Compose
-   Visual Studio 2022
-   Android Studio
-   Flutter SDK
-   .NET SDK
-   Stripe CLI



## 📥 Kloniranje projekta

git clone `<GITHUB_REPO_LINK>`


## 🔐 Konfiguracija

Fajl: Enviorment.7z
Šifra: fit

Izvaditi .env i staviti u root folder (GrooveOn).



## ▶️ Pokretanje Stripe

Instalirajte Stripe CLI:
https://docs.stripe.com/stripe-cli/install

Provjerite instalaciju:
stripe --version

Ulogujte se na Stripe dashboard:
https://dashboard.stripe.com/login

Email: testniadminmuzicar@gmail.com
Password: TestniMuzicar123!

U terminalu pokrenite:
stripe login

Pokrenite listener:
stripe listen --forward-to http://localhost:5277/api/payment/webhook

✔ Ako dobijete webhook secret → sve radi
⚠ Ostavite terminal upaljen tokom testiranja plaćanja


## ▶️ Pokretanje Dockera

docker compose up -d --build



## ▶️ Pokretanje aplikacije

Arhiva: IB220034-GrooveOn-Apps.7z\
Šifra: fit

Sadrži: 
- Release (desktop) folder u kojem je .exe 
- flutter-apk (mobilna) folder u kojem je .apk



## 🧪 Testni korisnici

Mobilna aplikacija: 
Username: fahrudinmusic11
Password: User123!

Desktop (Admin):
Username: dejanmusic01
Password: Admin123!



## ▶️ Testiranje email

Testiranje za desktop - dejanmusic01

- **Email:** `testniadminmuzicar@gmail.com`
- **Password:** `TestniAdminMuzicar123!`

Testiranje za mobile - fahrudinmusic11

- **Email:** `testnimuzicar@gmail.com`
- **Password:** `TestniMuzicar123!`

