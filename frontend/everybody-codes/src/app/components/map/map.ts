import { AfterViewInit, Component, Input, OnChanges, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Camera } from '../../models/camera';

@Component({
	selector: 'app-map',
	templateUrl: './map.html',
	styleUrl: './map.css'
})
export class MapComponent implements AfterViewInit, OnChanges, OnDestroy {
	@Input() cameras: Camera[] = [];
	private map: any;
	private markers: any;
	private L: any;

	constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

	ngAfterViewInit(): void {
		if (isPlatformBrowser(this.platformId)) {
			this.initializeMap();
		}
	}

	ngOnChanges(): void {
		if (isPlatformBrowser(this.platformId)) {
			this.reloadMarkers();
		}
	}

	ngOnDestroy(): void {
		if (this.map) {
			this.map.remove();
			this.map = undefined;
		}
	}

	private async initializeMap(): Promise<void> {
		try {
			this.L = await import('leaflet');

			const DefaultIcon = this.L.icon({
				iconUrl: 'assets/leaflet/marker-icon.png',
				iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
				shadowUrl: 'assets/leaflet/marker-shadow.png',
				iconSize: [25, 41],
				iconAnchor: [12, 41],
				popupAnchor: [1, -34],
				tooltipAnchor: [16, -28],
				shadowSize: [41, 41]
			});
			(this.L.Marker.prototype as any).options.icon = DefaultIcon;

			if (this.map) { this.map.remove(); this.map = undefined; }

			const container = document.getElementById('map') as any;
			if (container && container._leaflet_id) container._leaflet_id = null;

			this.map = this.L.map('map').setView([52.0914, 5.1115], 13);
			this.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
				attribution: '&copy; OpenStreetMap'
			}).addTo(this.map);

			this.markers = this.L.layerGroup();
			this.markers.addTo(this.map);

			setTimeout(() => this.map?.invalidateSize(), 0);

			this.reloadMarkers();
		} catch (error) {
			console.error('Failed to load map:', error);
		}
	}

	private reloadMarkers(): void {
		if (!this.map || !this.L) return;
		this.markers.clearLayers();
		this.cameras.forEach(c => {
			this.L.marker([c.latitude, c.longitude]).addTo(this.markers)
				.bindPopup(`${c.number} | ${c.code} ${c.name}`);
		});
	}
}
