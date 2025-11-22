<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="html" encoding="UTF-8" indent="yes" omit-xml-declaration="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Game Library</title>
				<style>
					* {
					margin: 0;
					padding: 0;
					box-sizing: border-box;
					}

					body {
					font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
					background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
					padding: 20px;
					min-height: 100vh;
					}

					.container {
					max-width: 1200px;
					margin: 0 auto;
					}

					h1 {
					color: white;
					text-align: center;
					margin-bottom: 30px;
					font-size: 3em;
					text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
					}

					.stats {
					background: white;
					border-radius: 15px;
					padding: 20px;
					margin-bottom: 30px;
					box-shadow: 0 10px 30px rgba(0,0,0,0.2);
					display: flex;
					justify-content: space-around;
					flex-wrap: wrap;
					}

					.stat-item {
					text-align: center;
					padding: 15px;
					}

					.stat-number {
					font-size: 2.5em;
					font-weight: bold;
					color: #667eea;
					}

					.stat-label {
					color: #666;
					margin-top: 5px;
					font-size: 0.9em;
					}

					.games-grid {
					display: grid;
					grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
					gap: 25px;
					}

					.game-card {
					background: white;
					border-radius: 15px;
					padding: 25px;
					box-shadow: 0 10px 30px rgba(0,0,0,0.2);
					transition: transform 0.3s ease, box-shadow 0.3s ease;
					position: relative;
					overflow: hidden;
					}

					.game-card:hover {
					transform: translateY(-10px);
					box-shadow: 0 15px 40px rgba(0,0,0,0.3);
					}

					.game-card::before {
					content: '';
					position: absolute;
					top: 0;
					left: 0;
					right: 0;
					height: 5px;
					background: linear-gradient(90deg, #667eea, #764ba2);
					}

					.game-title {
					font-size: 1.5em;
					color: #333;
					margin-bottom: 15px;
					font-weight: bold;
					}

					.game-badges {
					display: flex;
					gap: 8px;
					margin-bottom: 15px;
					flex-wrap: wrap;
					}

					.badge {
					display: inline-block;
					padding: 5px 12px;
					border-radius: 20px;
					font-size: 0.75em;
					font-weight: bold;
					text-transform: uppercase;
					}

					.badge-platform {
					background: #667eea;
					color: white;
					}

					.badge-rating {
					background: #f093fb;
					color: white;
					}

					.badge-multiplayer {
					background: #4facfe;
					color: white;
					}

					.game-info {
					margin: 10px 0;
					}

					.info-row {
					display: flex;
					margin: 8px 0;
					font-size: 0.95em;
					}

					.info-label {
					font-weight: bold;
					color: #667eea;
					min-width: 100px;
					}

					.info-value {
					color: #555;
					flex: 1;
					}

					.game-description {
					margin-top: 15px;
					padding-top: 15px;
					border-top: 1px solid #eee;
					color: #666;
					font-size: 0.9em;
					line-height: 1.6;
					}

					.game-price {
					margin-top: 15px;
					font-size: 1.8em;
					font-weight: bold;
					color: #667eea;
					text-align: right;
					}

					.price-free {
					color: #00d084;
					}

					.footer {
					text-align: center;
					color: white;
					margin-top: 50px;
					padding: 20px;
					font-size: 0.9em;
					}

					@media (max-width: 768px) {
					.games-grid {
					grid-template-columns: 1fr;
					}

					h1 {
					font-size: 2em;
					}

					.stats {
					flex-direction: column;
					}
					}
				</style>
			</head>
			<body>
				<div class="container">
					<h1>🎮 Game Library</h1>

					<!-- Statistics -->
					<div class="stats">
						<div class="stat-item">
							<div class="stat-number">
								<xsl:value-of select="count(//Game)"/>
							</div>
							<div class="stat-label">Total Games</div>
						</div>
						<div class="stat-item">
							<div class="stat-number">
								<xsl:value-of select="count(//Game[@platform='PC'])"/>
							</div>
							<div class="stat-label">PC Games</div>
						</div>
						<div class="stat-item">
							<div class="stat-number">
								<xsl:value-of select="count(//Game[@multiplayer='true'])"/>
							</div>
							<div class="stat-label">Multiplayer</div>
						</div>
						<div class="stat-item">
							<div class="stat-number">
								<xsl:value-of select="count(//Game[Genre='RPG' or contains(Genre, 'RPG')])"/>
							</div>
							<div class="stat-label">RPG Games</div>
						</div>
					</div>

					<!-- Games Grid -->
					<div class="games-grid">
						<xsl:apply-templates select="//Game"/>
					</div>

					<div class="footer">
						<p>
							Generated from XML using XSL | Total Games: <xsl:value-of select="count(//Game)"/>
						</p>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- Template for each Game -->
	<xsl:template match="Game">
		<div class="game-card">
			<div class="game-title">
				<xsl:value-of select="Title"/>
			</div>

			<div class="game-badges">
				<xsl:if test="@platform">
					<span class="badge badge-platform">
						<xsl:value-of select="@platform"/>
					</span>
				</xsl:if>

				<xsl:if test="@rating">
					<span class="badge badge-rating">
						<xsl:value-of select="@rating"/>
					</span>
				</xsl:if>

				<xsl:if test="@multiplayer='true'">
					<span class="badge badge-multiplayer">Multiplayer</span>
				</xsl:if>
			</div>

			<div class="game-info">
				<div class="info-row">
					<span class="info-label">Developer:</span>
					<span class="info-value">
						<xsl:value-of select="Developer"/>
					</span>
				</div>

				<div class="info-row">
					<span class="info-label">Publisher:</span>
					<span class="info-value">
						<xsl:value-of select="Publisher"/>
					</span>
				</div>

				<div class="info-row">
					<span class="info-label">Genre:</span>
					<span class="info-value">
						<xsl:value-of select="Genre"/>
					</span>
				</div>

				<div class="info-row">
					<span class="info-label">Release:</span>
					<span class="info-value">
						<xsl:value-of select="substring(ReleaseDate, 1, 10)"/>
					</span>
				</div>
			</div>

			<xsl:if test="Description">
				<div class="game-description">
					<xsl:value-of select="Description"/>
				</div>
			</xsl:if>

			<xsl:if test="Price">
				<div class="game-price">
					<xsl:choose>
						<xsl:when test="Price = '0.00' or Price = '0'">
							<span class="price-free">FREE</span>
						</xsl:when>
						<xsl:otherwise>
							$<xsl:value-of select="Price"/>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</xsl:if>
		</div>
	</xsl:template>

</xsl:stylesheet>