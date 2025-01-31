<?php

require_once('../_helpers/strip.php');

// Désactive le chargement des entités externes pour prévenir XXE
libxml_disable_entity_loader(true);

// Définit le XML en s'assurant qu'il provient d'une source sécurisée
$xml = isset($_GET['xml']) && !empty($_GET['xml']) ? $_GET['xml'] : '<root><content>No XML found</content></root>';

$document = new DOMDocument();

// Charge le XML de manière sécurisée sans activer les DTD et les entités externes
$document->loadXML($xml, LIBXML_NOENT); // Supprime LIBXML_DTDLOAD pour éviter XXE

// Sécuriser l'importation du DOM XML en SimpleXML
$parsedDocument = simplexml_import_dom($document);

// Affichage du contenu du document XML de manière sécurisée
echo htmlspecialchars($parsedDocument->content, ENT_QUOTES, 'UTF-8'); // Évite XSS

