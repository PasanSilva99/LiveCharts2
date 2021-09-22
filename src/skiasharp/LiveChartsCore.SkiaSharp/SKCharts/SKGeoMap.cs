﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts
{
    /// <inheritdoc cref="IGeoMapView{SkiaSharpDrawingContext}"/>
    public class SKGeoMap : IGeoMapView<SkiaSharpDrawingContext>, ISkiaSharpChart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
        /// </summary>
        public SKGeoMap()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
        /// </summary>
        /// <param name="mapView">The map view.</param>
        public SKGeoMap(IGeoMapView<SkiaSharpDrawingContext> mapView)
        {
            MapProjection = mapView.MapProjection;
            HeatMap = mapView.HeatMap;
            ColorStops = mapView.ColorStops;
            Stroke = mapView.Stroke;
            Fill = mapView.Fill;
            Shapes = mapView.Shapes;
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Measured"/>
        public event Action<IGeoMapView<SkiaSharpDrawingContext>>? Measured;

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public SKColor Background { get; set; } = SKColors.White;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; } = 600;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; } = 900;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
        public object SyncContext { get; set; } = new object();

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
        public bool DesignerMode { get; set; } = false;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
        public GeoJsonFile ActiveMap { get; set; } = Maps.GetWorldMap();

        float IGeoMapView<SkiaSharpDrawingContext>.Width => Width;

        float IGeoMapView<SkiaSharpDrawingContext>.Height => Height;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas { get; } = new();

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
        public MapProjection MapProjection { get; set; }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.HeatMap"/>
        public LvcColor[] HeatMap { get; set; } = new[]
        {
            LvcColor.FromArgb(255, 179, 229, 252), // cold (min value)
            LvcColor.FromArgb(255, 2, 136, 209) // hot (max value)
        };

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ColorStops"/>
        public double[]? ColorStops { get; set; }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
        public IPaint<SkiaSharpDrawingContext>? Stroke { get; set; } = new SolidColorPaint(new SKColor(255, 224, 224, 224));

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
        public IPaint<SkiaSharpDrawingContext>? Fill { get; set; } = new SolidColorPaint(new SKColor(255, 250, 250, 250));

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Shapes"/>
        public IEnumerable<MapShape<SkiaSharpDrawingContext>> Shapes { get; set; } = Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>();

        /// <inheritdoc cref="ISkiaSharpChart.GetImage"/>
        public SKImage GetImage()
        {
            load();
            Canvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            Canvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    Canvas,
                    new SKImageInfo(Height, Width),
                    surface,
                    canvas)
                {
                    ClearColor = Background
                });

            return surface.Snapshot();
        }

        /// <inheritdoc cref="ISkiaSharpChart.SaveImage(string, SKEncodedImageFormat, int)"/>
        public void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
        {
            load();
            Canvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            Canvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    Canvas,
                    new SKImageInfo(Height, Width),
                    surface,
                    canvas)
                {
                    ClearColor = Background
                });

            using var image = surface.Snapshot();
            using var data = image.Encode(format, quality);
            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }

        void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
        {
            action();
        }

        private void load()
        {
            //var paint = new SolidColorPaint();

            //var thickness = (float)StrokeThickness;
            //var stroke = LvcColor.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            //var fill = LvcColor.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            //var hm = HeatMap.Select(x => LvcColor.FromArgb(x.A, x.R, x.G, x.B)).ToArray();

            //if (_heatKnownLength != HeatMap.Length)
            //{
            //    _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
            //    _heatKnownLength = HeatMap.Length;
            //}

            //var worldMap = s_map ??= Maps.GetWorldMap();
            //var projector = Maps.BuildProjector(Projection, new float[] { Width, Height });
            //var shapes = worldMap.AsMapShapes(hm, _heatStops, stroke, fill, thickness, projector);
            //paint.SetGeometries(Canvas, new HashSet<IDrawable<SkiaSharpDrawingContext>>(shapes));
            //var tasks = new HashSet<IPaint<SkiaSharpDrawingContext>> { paint };
            //Canvas.SetPaintTasks(tasks);

            //Measured?.Invoke(this);
        }
    }
}
