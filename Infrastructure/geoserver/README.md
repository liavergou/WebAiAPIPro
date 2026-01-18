# Εγκατάσταση GeoServer (Manual)

**Στόχος**: Ρύθμιση του GeoServer για την υποστήριξη του layer `ConversionJobsView`, σύνδεση με βάση και ρύθμισεις Authentication.

## Προαπαιτούμενα
1.  **GeoServer 2.28.1**: [Download](https://geoserver.org/release/2.28.1/) (Επιλογή **Platform Independent Binary**).
2.  **PostgreSQL 18 + PostGIS**: Βεβαιωθείτε ότι υπάρχει η βάση `coordextractordb` και έχει τρέξει το command `CREATE EXTENSION postgis;`.
3.  **Port 8085**: Θα ρυθμιστεί στο αρχείο `start.ini`.

## Εγκατάσταση

### 1. Εξαγωγή & Φάκελοι
1.  Κάντε extract το zip του GeoServer στο `C:\GeoServer_2.28.1_Install` (ή όπου επιθυμείτε).
2.  Δημιουργήστε έναν φάκελο για τα δεδομένα: `C:\GeoServer_data`.

### 2. Ρύθμιση Environment Variable
Ορίστε τον φάκελο δεδομένων στα Windows System Variables:
*   **Variable Name**: `GEOSERVER_DATA_DIR`
*   **Variable Value**: `C:\GeoServer_data`

### 3. Αντιγραφή Configuration Αρχείων
Αντιγράψτε τα περιεχόμενα του φακέλου του project `Infrastructure\geoserver\` (εκτός από αυτό το README) μέσα στον φάκελο `C:\GeoServer_data`.
*   Θα πρέπει να υπάρχουν φάκελοι όπως `workspaces`, `security`, `styles`, και αρχεία `xml` μέσα στο `C:\GeoServer_data`.

### 4. Ρύθμιση Port (8085)
Ανοίξτε το αρχείο `C:\GeoServer_2.28.1_Install\start.ini` και ρυθμίστε:
```ini
## Connector port to listen on
jetty.http.port=8085
```

### 5. Εκκίνηση
Εκτελέστε το script:
`C:\GeoServer_2.28.1_Install\bin\startup.bat`

## Έλεγχος Λειτουργίας

### 1. Πρόσβαση
Ανοίξτε: `http://localhost:8085/geoserver`
*   **User**: `admin`
*   **Password**: `geoserver`

### 2. Σύνδεση με Βάση (Data Store)
Ελέγξτε ότι το Store `conversion_jobs_store` είναι συνδεδεμένο:
1.  Πηγαίνετε **Stores** -> `conversion_jobs_store`.
2.  Επαληθεύστε τα στοιχεία σύνδεσης (Host, Port, DB, User, Pass).
3.  Πατήστε **Save**.

### 3. Επαλήθευση Layer και SQL View
Βεβαιωθείτε ότι το Layer αναγνωρίστηκε σωστά:
1.  Πηγαίνετε **Layers** -> `ConversionJobsView`.
2.  Στην καρτέλα **Data** στο edit sql view, ελέγξτε ότι η υπάρχει το SQL καθώς και στα Feature Type Detailes στο Details υπάρχει το υπολογιζόμενο πεδίο Area με τιμή round(area(Geom)*100)/100.0

### 4. Επαλήθευση Authentication
Ελέγξτε ότι το Keycloak Authentication είναι ενεργό:
1.  Στο αριστερό μενού, επιλέξτε **Security** -> **Authentication**.
2.  Στον πίνακα **Authentication Filters** ,βεβαιωθείτε ότι υπάρχει το φίλτρο:
    *   **Name**: `Keycloak-Auth`

### 5. WFS Test
Δοκιμάστε το παρακάτω URL για να δείτε αν επιστρέφει JSON δεδομένα:
```
http://localhost:8085/geoserver/wfs?service=WFS&request=GetFeature&typeName=topo_app:ConversionJobsView&outputFormat=application/json&srsName=EPSG:4326