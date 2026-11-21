<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<title>Список мешканців гуртожитку</title>
				<style>
					body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f7fafc; padding: 25px; }
					h1 { color: #2b6cb0; border-bottom: 2px solid #e2e8f0; padding-bottom: 10px; }
					p.subtitle { color: #4a5568; font-size: 14px; margin-bottom: 20px; }
					table { width: 100%; border-collapse: collapse; background: white; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border-radius: 8px; overflow: hidden; }
					th { background-color: #3182ce; color: white; text-align: left; padding: 12px 15px; font-weight: 600; }
					td { padding: 10px 15px; border-bottom: 1px solid #edf2f7; color: #2d3748; }
					tr:hover { background-color: #ebf8ff; }
					.badge { padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }
					.badge-head { background-color: #feebc8; color: #c05621; }
					.badge-benefit { background-color: #fed7d7; color: #9b2c2c; }
				</style>
			</head>
			<body>
				<h1>Список мешканців гуртожитку</h1>
				<p class="subtitle">Дані сформовано системою «ІС Гуртожиток»</p>

				<table>
					<thead>
						<tr>
							<th>П.І.Б. Студента</th>
							<th>Факультет</th>
							<th>Кафедра / Спеціальність</th>
							<th>Кімната</th>
							<th>Курс</th>
							<th>Дата поселення</th>
							<th>Примітки / Пільги</th>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select="Dormitory/Student">
							<tr>
								<td>
									<strong>
										<xsl:value-of select="@fullName"/>
									</strong>
								</td>
								<td>
									<xsl:value-of select="@faculty"/>
								</td>
								<td>
									<xsl:value-of select="@department"/>
								</td>
								<td>
									<xsl:value-of select="@roomNumber"/>
								</td>
								<td>
									<xsl:value-of select="@course"/> курс
								</td>
								<td>
									<xsl:value-of select="@checkInDate"/>
								</td>
								<td>
									<xsl:if test="@isRoomHead = 'true'">
										<span class="badge badge-head">Староста кімнати</span>
										<xsl:text> </xsl:text>
									</xsl:if>
									<xsl:if test="@benefit">
										<span class="badge badge-benefit">
											<xsl:value-of select="@benefit"/>
										</span>
									</xsl:if>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>