import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Camera } from '../../models/camera';
import { CameraService } from '../../services/camera.service';
import { MapComponent } from '../map/map';

@Component({
	selector: 'app-cameras',
	imports: [FormsModule, CommonModule, MapComponent],
	templateUrl: './cameras.html',
	styleUrl: './cameras.css'
})
export class CamerasComponent implements OnInit {
	q = '';
	cameras: Camera[] = [];
	col1: Camera[] = [];
	col2: Camera[] = [];
	col3: Camera[] = [];
	col4: Camera[] = [];

	constructor(private cameraService: CameraService) { }

	ngOnInit(): void {
		this.load();
	}

	load(): void {
		this.cameraService.getAll().subscribe(list => {
			this.cameras = list;
			this.splitColumns(list);
		});
	}

	searchByName(name: string): void {
		if (!name) {
			this.load();
			return;
		}

		name = name.trim();
		this.cameraService.search(name).subscribe(data => {
			this.cameras = data;
			this.splitColumns(data);
		});
	}

	private splitColumns(cameras: Camera[]): void {
		this.col1 = [];
		this.col2 = [];
		this.col3 = [];
		this.col4 = [];

		for (const camera of cameras) {
			const col = classifyColumn(camera.number);
			if (col === 1) this.col1.push(camera);
			else if (col === 2) this.col2.push(camera);
			else if (col === 3) this.col3.push(camera);
			else this.col4.push(camera);
		}
	}
}

export function classifyColumn(n: number): 1 | 2 | 3 | 4 {
	const div3 = n % 3 === 0;
	const div5 = n % 5 === 0;
	if (div3 && div5) return 3;
	if (div3) return 1;
	if (div5) return 2;
	return 4;
}
