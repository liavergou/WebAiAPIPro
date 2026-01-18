# Εγκατάσταση Keycloak (Docker)

**Στόχος**: Εγκατάσταση Keycloak με ρυθμισμένο Realm (`TopoApp`), Clients (`web-api`, `react-app`), και Roles.

## Προαπαιτούμενα
- **Docker Desktop** (εγκατεστημένο και ενεργό)
- **Port 8080** ελεύθερη

## Αρχεία
- `docker-compose.yml`: Το αρχείο ορισμού του container.
- `realm-export.json`: Το configuration του Realm που θα γίνει import.

## Βήματα Εγκατάστασης

### 1. Ρύθμιση Environment (`docker-compose.yml`)
Ανοίξτε το `docker-compose.yml` και ελέγξτε τις μεταβλητές:
```yaml
environment:
  POSTGRES_USER: <YOUR_DB_USER>       # Χρήστης βάσης Keycloak
  POSTGRES_PASSWORD: <YOUR_DB_PASSWORD> # Κωδικός βάσης Keycloak
  KEYCLOAK_ADMIN: admin               # Admin χρήστης Keycloak Console
  KEYCLOAK_ADMIN_PASSWORD: admin      # Admin κωδικός Keycloak Console
  # Για Production, αλλάξτε το localhost σε Static IP
  KC_HOSTNAME: localhost 
```

### 2. Προετοιμασία Import
Για να διαβάσει το Keycloak το αρχείο `realm-export.json` κατά την εκκίνηση:
1.  Δημιουργήστε έναν φάκελο `imports` στον φάκελο που βρίσκεται το `docker-compose.yml`.
2.  Αντιγράψτε το αρχείο `realm-export.json` μέσα στον φάκελο `imports`.
    ```bash
    mkdir imports
    copy realm-export.json imports\
    ```
### 3. Εκκίνηση
Εκτελέστε την εντολή:
```bash
docker-compose -f docker-compose.yml up -d
```
### 4. Αντικατάσταση Client Secret
Για να συνδεθεί το Backend (`web-api`) με το Keycloak:

1.  Ανοίξτε: `http://localhost:8080`
2.  Login στο **Administration Console**:
    *   User: `admin`
    *   Pass: `admin` (ή ότι ορίσατε στο docker-compose)
3.  Επιλέξτε το Realm **TopoApp**
4.  Πηγαίνετε: **Clients** -> **web-api** -> καρτέλα **Credentials**.
5.  Πατήστε το εικονίδιο **Copy** στο **Client Secret**.
6.  Επικολλήστε το στο `appsettings.Development.json` του Backend:
    ```json
    "Keycloak": {
      "AdminApi": {
        "ClientSecret": "<CLIENT_SECRET>"
      }
    }
    ```