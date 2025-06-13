# Manual Tècnic de Realitat Virtual Multijugador

## Taula de Continguts
1. [Visió General del Projecte](#visió-general-del-projecte)
2. [Configuració i Prerequisits](#configuració-i-prerequisits)
3. [Configuració de OpenXR](#configuració-de-openxr)
4. [Configuració de la Xarxa Photon](#configuració-de-la-xarxa-photon)
5. [Estructura del Projecte](#estructura-del-projecte)
6. [Detalls d'Implementació](#detalls-dimplementació)
7. [Arquitectura de Xarxa](#arquitectura-de-xarxa)
8. [Problemes Coneguts i Solucions](#problemes-coneguts-i-solucions)
9. [Millors Pràctiques](#millors-pràctiques)

## Visió General del Projecte
Aquest projecte implementa una experiència de realitat virtual multijugador utilitzant Unity i Photon PUN2 (Photon Unity Networking 2). L'aplicació permet a múltiples usuaris interactuar en un espai virtual compartit, manipulant objectes i col·laborant en temps real. El projecte està dissenyat específicament per a les plataformes Meta Quest 3 i Pico 4, oferint una experiència immersiva i interactiva en aquests dispositius mitjançant OpenXR.

### Característiques Principals
- Entorn VR multijugador amb 2 jugadors
- Sincronització d'objectes en temps real
- Gestió de sincronia entre objectes
- Components de trencaclosques interactius
- Sistema de registre d'esdeveniments
- Suport específic per a Meta Quest 3 i Pico 4 via OpenXR
- Integració multiplataforma amb Photon PUN2

## Configuració i Prerequisits

### Programari Requerit
1. Unity 2022.3 LTS o més recent, en el projecte s'ha utilitzat [2022.3.47f1](https://unity.com/releases/editor/whats-new/2022.3.47)
2. [Visual Studio](https://visualstudio.microsoft.com/es/#vs-section) (o IDE preferit)
3. [SDK XR Interaction Toolkit](https://docs.unity3d.com/es/2019.4/Manual/com.unity.xr.interaction.toolkit.html)
4. [PICO Unity OpenXR SDK](https://developer.picoxr.com/resources/)
5. [Meta XR Core SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-core-sdk-269169)
6. [PUN2 Unity SDK](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922?srsltid=AfmBOoo7uXE8BpnhabcW1OQrZva9lyuI4CNfdjJOlANw5SrBkFYPiGf2)

### Dependències de Paquets Unity
- XR Interaction Toolkit
- XR Plugin Management
- Photon PUN2
- OpenXR Plugin
- Meta XR Core SDK

### Requisits de Maquinari
1. **Meta Quest 3**
   - Resolució: 2064 x 2208 per ull
   - Camp de visió: 110° horitzontal
   - Freqüència de refresc: 90Hz/120Hz
   - Controladors: Touch Plus amb seguiment de dits
   - Perfil OpenXR: Oculus Touch Controller Profile

2. **Pico 4**
   - Resolució: 2160 x 2160 per ull
   - Camp de visió: 105° horitzontal
   - Freqüència de refresc: 90Hz
   - Controladors: Pico 4 Sense amb seguiment de dits
   - Perfil OpenXR: PICO4 Touch Controller Profile

## Configuració de OpenXR

### 1. Importació de SDK necessaris
1. Importa el [SDK de OpenXR de PICO](https://developer.picoxr.com/resources/)
2. Importa el [SDK de Meta Core](https://assetstore.unity.com/packages/tools/integration/meta-xr-core-sdk-269169)

### 2. Configuració de _Project Settings_
1. Anant a _Editar > Configuració del Projecte > Gestió de Connectors XR_.
2. A la secció de _Android_, a _Proveidors de Connectors_, habilita _OpenXR_.

### 3. Configuració de OpenXR amb diferents HMD
1. Depenent de les HMD on es vulgui posar l'aplicació, s'ha de configurar _OpenXR_ per a cada HMD per separat. Les diferents documentacions: [Pico](https://developer.picoxr.com/document/unity-openxr/about-openxr-and-the-unity-engine/) i [Meta](https://docs.unity3d.com/Packages/com.unity.xr.meta-openxr@2.0/manual/index.html)
2. Per configurar això, s'ha d'anar a la secció _Edit > Project Settings > XR Plug-in Management_, i al menú de _Perfils d'Interacció_ s'han d'afegir únicament els perfils que es vagin a utilitzar.
   - Per a les Meta s'ha d'afegir _Perfil de Controlador Oculus Touch_
   - Per a les Pico s'ha d'afegir _Perfil de Controlador PICO4 Touch_
   - Per a altres proveidors de HMD, s'ha de consultar la documentació del SDK de OpenXR de l'HMD específic.
3. Un altre punt a habilitar són els _Grups de Característiques OpenXR_, que, similar a l'anterior, s'ha de fer per separat depenent de les HMD que es vagin a utilitzar.
   - Per a les Meta s'ha d'habilitar **únicament** l'opció de _Meta Quest_
   - Per a les Pico s'ha d'habilitar **únicament** l'opció de _Pico XR_

## Configuració de la Xarxa Photon

### 1. Creació d'un Compte Photon
1. Visita la [pàgina web de Photon](https://www.photonengine.com/)
2. Crea un compte nou
3. Navega al Tauler de Control de Photon
4. Crea una nova aplicació
5. Selecciona "Photon PUN" com a tipus
6. Anota el teu App ID

### 2. Configuració del Projecte PUN2
1. Importa PUN2 des de l'[Asset Store](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922?srsltid=AfmBOoo7uXE8BpnhabcW1OQrZva9lyuI4CNfdjJOlANw5SrBkFYPiGf2)
2. Obre l'Assistent PUN (Window > Photon Unity Networking > Highlight Server Settings)
3. Introdueix el teu App ID a la Configuració del Servidor
4. Configura els següents paràmetres:
   - App Id PUN: El teu Photon App ID
   - Versió de l'App: Estableix-la per coincidir amb la versió del teu joc
   - Regió: Selecciona la més propera als teus usuaris objectiu


## Configuració de la App de Pico

### 1. Obtenció de l'AppId
1. Visita el [PICO Developer Console](https://developer-global.picoxr.com/console/app/home)
2. Crea un compte nou o inicia sessió
3. Crea una nova aplicació:
   - Selecciona "Crear Aplicació"
   - Omple el nom i la descripció bàsica
   - Selecciona "VR" com a tipus d'aplicació
4. Un cop creada, trobaràs l'AppId a la secció "Configuració de l'Aplicació"

### 2. Configuració a Unity
1. Al projecte Unity:
   - Obre "Project Settings > XR Plug-in Management"
   - A la secció "Android", habilita "OpenXR"
   - A "OpenXR Feature Groups", habilita "Pico XR"
2. Configura l'AppId:
   - Obre "Project Settings > Player"
   - A la secció "Android", configura el "Package Name" segons el format: `com.teva_organitzacio.nom_app`
   - Assegura't que aquest identificador coincideix amb el de l'aplicació al PICO Developer Console


## Estructura del Projecte

### Components Principals
```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── SceneController.cs       # Gestiona la generació i estat de l'escena
│   │   └── SessionManager.cs        # Gestiona la sessió global
│   ├── Network/
│   │   ├── NetworkManager.cs        # Controlador principal de xarxa
│   │   ├── PlayerVRManager.cs       # Sincronització de jugadors VR
│   │   ├── NetworkInteractableObject.cs # Interacció d'objectes en xarxa
│   │   ├── OwnershipManager.cs      # Gestió de propietat d'objectes
│   │   └── EventManager.cs          # Sistema d'esdeveniments de xarxa
│   ├── Interaction/
│   │   └── SnapSlot.cs             # Sistema d'encaixament d'objectes
│   └── TableLogic/
│       └── TablePlacer.cs          # Col·locació de components del trencaclosques
```

## Detalls d'Implementació

### Arquitectura de Xarxa

#### Gestió de Jugadors
- Cada jugador s'instancia com a prefab en xarxa
- Les posicions i rotacions dels jugadors es sincronitzen utilitzant PhotonTransformView
- Es desactiven els visuals locals per evitar renderitzat doble
- Els jugadors remots es representen amb avatars en xarxa
- Suport específic per a controladors OpenXR de cada plataforma

#### Sincronització d'Objectes
1. **Sistema de Propietat**
   - Els objectes utilitzen `OwnershipOption.Request` per transferència
   - Es demana propietat en agafar objectes
   - Les transferències es gestionen per `OwnershipManager`
   - Gestió específica per a interaccions OpenXR

2. **Esdeveniments de Xarxa**
   - Sistema d'esdeveniments personalitzat per gestió d'escenes
   - Codis d'esdeveniment per:
     - Creació d'escena
     - Finalització d'escena
     - Interacció amb objectes
     - Accions de jugador
     - Esdeveniments específics d'OpenXR

3. **Gestió de Sales**
   - Màxim 2 jugadors per sala
   - El client mestre controla l'estat de l'escena
   - Sincronització automàtica d'escenes
   - Suport per a diferents perfils de controlador OpenXR

### Implementació VR

#### Configuració del Jugador
1. **Estructura XR Rig**
   - Seguiment del cap amb suport específic per a Quest 3 i Pico 4 via OpenXR
   - Controladors de mans amb seguiment de dits segons el perfil OpenXR
   - Sincronització en xarxa de tots els components
   - Calibració específica per a cada dispositiu

2. **Sistema d'Interacció**
   - Integració amb XR Interaction Toolkit i OpenXR
   - Sistema d'agafament en xarxa
   - Sistema d'encaixament amb validació
   - Suport per a gestos específics de cada dispositiu via OpenXR

#### Optimitzacions Específiques per Dispositiu
1. **Meta Quest 3**
   - Optimització de rendiment per a 120Hz
   - Suport per a seguiment de dits avançat via OpenXR
   - Gestió de memòria específica per a GPU Adreno 740
   - Aprofitament de la resolució nativa
   - Configuració específica del perfil Oculus Touch

2. **Pico 4**
   - Optimització per a pantalla LCD de 2160x2160
   - Gestió de memòria específica per a Snapdragon XR2
   - Suport per a seguiment de dits Pico via OpenXR
   - Ajustaments específics per a controladors Pico 4 Sense
   - Configuració específica del perfil PICO4 Touch

## Problemes Coneguts i Solucions

### 1. Problemes de Transferència de Propietat
**Problema**: Els objectes poden no transferir la propietat correctament
**Solució**: 
- Assegurar que `OwnershipOption.Request` està configurat
- Implementar una gestió adequada de sol·licituds de propietat
- Utilitzar `PhotonView.RequestOwnership()` en agafar
- Verificar la compatibilitat amb els perfils OpenXR

### 2. Latència de Xarxa
**Problema**: El moviment dels objectes pot semblar entrecortat
**Solució**:
- Utilitzar PhotonTransformView amb interpolació
- Implementar configuracions de física adequades
- Considerar l'ús de `Rigidbody.isKinematic` mentre l'usuari no té la propietat de l'objecte.
- Ajustar la interpolació segons el dispositiu (90Hz/120Hz)
- Optimitzar per a diferents perfils de controlador OpenXR

### 3. Sincronització d'Escenes
**Problema**: L'estat de l'escena pot desincronitzar-se entre clients
**Solució**:
- Utilitzar `PhotonNetwork.AutomaticallySyncScene = true`
- Implementar una seqüència adequada de càrrega d'escenes
- Utilitzar esdeveniments de xarxa per canvis d'estat
- Gestionar diferents configuracions OpenXR entre clients

### 4. Problemes Específics de Dispositiu
**Problema**: Diferències en el rendiment i interaccions entre Quest 3 i Pico 4
**Solució**:
- Implementar LOD dinàmic basat en el dispositiu
- Ajustar la qualitat gràfica segons el dispositiu
- Optimitzar les textures per a cada resolució nativa
- Gestionar diferents freqüències de refresc
- Configurar correctament els perfils OpenXR per cada dispositiu
- Implementar gestió específica per a cada tipus de controlador

## Millors Pràctiques

### Optimització de Xarxa
1. **Agrupament d'Objectes**
   - Implementar agrupament d'objectes per objectes generats freqüentment
   - Reutilitzar IDs de xarxa quan sigui possible
   - Optimitzar per a diferents perfils de controlador OpenXR

2. **Ús d'Esdeveniments**
   - Utilitzar esdeveniments fiables per operacions crítiques
   - Implementar cache d'esdeveniments adequat
   - Netejar listeners d'esdeveniments
   - Gestionar esdeveniments específics d'OpenXR

3. **Gestió de Propietat**
   - Demanar propietat només quan sigui necessari
   - Implementar validació adequada de transferència de propietat
   - Gestionar fallades en transferències de propietat
   - Considerar les limitacions dels diferents perfils de controlador

### Millors Pràctiques VR
1. **Rendiment**
   - Optimitzar el trànsit de xarxa
   - Optimitzar per a cada perfil OpenXR

2. **Experiència d'Usuari**
   - Proporcionar retroalimentació visual per operacions de xarxa
   - Implementar gestió adequada d'errors
   - Afegir estats de càrrega per operacions de xarxa
   - Adaptar la interfície per a cada dispositiu
   - Gestionar diferents tipus de controlador

### Consideracions de Seguretat
1. **Gestió de Sales**
   - Implementar validació adequada de sales
   - Utilitzar propietats de sala per l'estat del joc
   - Validar accions de jugador
   - Verificar compatibilitat de dispositius

2. **Validació de Dades**
   - Validar tots els esdeveniments de xarxa
   - Implementar gestió adequada d'errors
   - Utilitzar funcions de seguretat incorporades de Photon
   - Verificar integritat dels perfils OpenXR

## Recursos Addicionals
- [Documentació de Photon PUN2](https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro)
- [Eina d'Interacció XR d'Unity](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest)
- [Millors Pràctiques de Photon](https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/player-networking)
- [Documentació OpenXR de Pico](https://developer.picoxr.com/document/unity-openxr/about-openxr-and-the-unity-engine/)
- [Documentació OpenXR de Meta](https://docs.unity3d.com/Packages/com.unity.xr.meta-openxr@2.0/manual/index.html) 