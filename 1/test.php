<?php

require_once('../_helpers/strip.php');

// Désactive le chargement des entités externes pour prévenir XXE
libxml_disable_entity_loader(true);

// Vérification et définition d'un XML sécurisé
$xml = isset($_GET['xml']) && !empty($_GET['xml']) ? $_GET['xml'] : '<root><content>No XML found</content></root>';

$document = new DOMDocument();

// Charge le XML de manière sécurisée en empêchant l'accès aux ressources externes
$document->loadXML($xml, LIBXML_NONET); // Remplacement de LIBXML_NOENT par LIBXML_NONET

// Sécurisation de l'importation XML
$parsedDocument = simplexml_import_dom($document);

// Affichage sécurisé pour éviter XSS
echo htmlspecialchars($parsedDocument->content, ENT_QUOTES, 'UTF-8');
