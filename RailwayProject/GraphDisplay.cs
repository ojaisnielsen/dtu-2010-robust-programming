using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using RailwayProject;
using System.Diagnostics.Contracts;

namespace GraphDisplay
{
    static class Display
    {
        class Node
        {            
            List<Node> neighbours = new List<Node>();
            Vector position = new Vector(0, 0);
            Vector speed = new Vector(0, 0);
            double radius;
            string label;

            [ContractInvariantMethod]
            private void NodeObjectInvariant()
            {
                Contract.Invariant(this.neighbours != null);
                Contract.Invariant(this.position != null);
                Contract.Invariant(this.speed != null);
                Contract.Invariant(this.radius >= 0);
                Contract.Invariant(this.neighbours.All((Node node) => node != null));
                Contract.Invariant(!this.neighbours.Contains(this));
            }

            public Node(double radius, string label)
            {
                this.radius = radius;
                this.label = label;
                this.Position = new Vector(0, 0);
            }

            public Vector Position
            {
                get
                {
                    return this.position;
                }
                set
                {
                    this.position = value;
                }
            }
            public Vector Speed
            {
                get
                {
                    return this.speed;
                }
            }

            public List<Node> Neighbours
            {
                get
                {
                    return this.neighbours;
                }
            }

            public string Label
            {
                get
                {
                    return this.label;
                }
            }

            public double Radius
            {
                get
                {
                    return this.radius;
                }
            }

            public void Move(Vector force)
            {
                Contract.Requires(force != null);
                this.speed = this.speed + force;
                this.Position = this.position + this.speed;
            }

            public void Place(Vector position)
            {
                this.speed = new Vector(0, 0);
                this.Position = position;
            }

            public void AddNeighbour(Node node)
            {
                Contract.Requires(node != null);
                if (!this.neighbours.Contains(node))
                {
                    this.neighbours.Add(node);
                }
            }

            public static double Distance(Node node1, Node node2)
            {
                Contract.Requires(node1 != null);
                Contract.Requires(node2 != null);
                return (node1.Position - node2.Position).Length - node1.Radius - node2.Radius;
            }
        }

        class Graph
        {
            List<Node> nodes = new List<Node>();
            List<Node> startNodes = new List<Node>();
            List<Node> endNodes = new List<Node>();
            List<int> nArrows = new List<int>();
            public readonly double distance;

            [ContractInvariantMethod]
            private void GraphObjectInvariant()
            {
                Contract.Invariant(this.nodes != null);
                Contract.Invariant(this.nodes.All((Node node) => node != null));
                Contract.Invariant(this.nodes.Count == this.nodes.Distinct().Count());
                Contract.Invariant(this.startNodes != null);
                Contract.Invariant(this.startNodes.All((Node node) => node != null));                
                Contract.Invariant(this.endNodes != null);
                Contract.Invariant(this.endNodes.All((Node node) => node != null));                
                Contract.Invariant(this.nArrows != null);
                Contract.Invariant(this.endNodes.Count == this.startNodes.Count);
                Contract.Invariant(this.endNodes.Count == this.nArrows.Count);
                Contract.Invariant(Contract.ForAll(0, this.startNodes.Count, (int n) => this.startNodes[n] != this.endNodes[n]));
                Contract.Invariant(this.startNodes.All((Node node) => this.nodes.Contains(node)));
                Contract.Invariant(this.endNodes.All((Node node) => this.nodes.Contains(node)));
                Contract.Invariant(this.nodes.All((Node node) => this.distance >= 2 * node.Radius));
                Contract.Invariant(this.nArrows.All((int nArrow) => nArrow >= 0 && nArrow <= 2));   
            }

            public List<Node> Nodes
            {
                get
                {
                    return nodes;
                }
            }

            public double Width
            {                
                get
                {
                    Contract.Ensures(Contract.Result<Double>() >= 0);
                    double minX = this.nodes.Aggregate(Double.PositiveInfinity, (double min, Node node) => Math.Min(min, node.Position.X - node.Radius));
                    double maxX = this.nodes.Aggregate(0, (double max, Node node) => Math.Max(max, node.Position.X + node.Radius));
                    return maxX - minX;
                }
            }

            public double Height
            {
                get
                {
                    Contract.Ensures(Contract.Result<Double>() >= 0);
                    double minY = this.nodes.Aggregate(Double.PositiveInfinity, (double min, Node node) => Math.Min(min, node.Position.Y - node.Radius));
                    double maxY = this.nodes.Aggregate(0, (double max, Node node) => Math.Max(max, node.Position.Y + node.Radius));
                    return maxY - minY;
                }
            }
            
            public Graph(double distance)
            {
                this.distance = distance;
            }

            public void AddNode(Node node)
            {
                Contract.Requires(node != null);
                if (!this.nodes.Contains(node))
                {
                    this.nodes.Add(node);
                }
            }

            public void Link(Node node1, Node node2, int nArrows)
            {
                Contract.Requires(node1 != null);
                Contract.Requires(node2 != null);
                if (this.nodes.Contains(node1) && this.nodes.Contains(node2))
                {
                    node1.AddNeighbour(node2);
                    node2.AddNeighbour(node1);
                    this.startNodes.Add(node1);
                    this.endNodes.Add(node2);
                    this.nArrows.Add(nArrows);
                }
            }

            public void Initiate()
            {
                Contract.Ensures(this.nodes.All((Node node1) => nodes.All((Node node2) => (node1.Position - node2.Position).Length >= this.distance || node1 == node2)));
                int m = (int)Math.Sqrt(this.nodes.Count) + 1;
                for (int n = 0; n < this.nodes.Count; n++)
                {
                    this.nodes[n].Position = new Vector((n / m) * this.distance, (n % m) * this.distance);
                }
            }

            public void Center()
            {
                Vector massCenter = this.nodes.Aggregate(new Vector(0, 0), (Vector vect, Node node) => vect + (node.Position / this.nodes.Count));
                Vector center = new Vector(this.Width / 2, this.Height / 2);
                this.nodes.ForEach((Node node) => node.Position = node.Position - massCenter + center);
            }          

            public Canvas Draw()
            {
                Contract.Ensures(Contract.Result<Canvas>() != null);
                Contract.Ensures(Contract.Result<Canvas>().Children.OfType<Ellipse>().Count() == this.nodes.Where((Node node) => node.Radius > 0).Count());
                Contract.Ensures(Contract.Result<Canvas>().Children.OfType<TextBlock>().Count() == this.nodes.Where((Node node) => node.Radius > 0).Count());
                Contract.Ensures(Contract.Result<Canvas>().Children.OfType<Polygon>().Count() == this.startNodes.Count);

                Canvas canvas = new Canvas();
                canvas.Height = this.Height;
                canvas.Width = this.Width;

                this.Center();

                foreach (Node node in this.nodes)
                {
                    if (node.Radius > 0)
                    {
                        canvas.Children.Add(DrawNode(node));
                        canvas.Children.Add(DrawLabel(node));
                    }
                }

                double[] offsets = new double[this.startNodes.Count];
                foreach (Node node1 in this.startNodes.Distinct())
                {
                    foreach (Node node2 in this.endNodes.Distinct())
                    {
                        List<int> currentEdgeIndices = Enumerable.Range(0, this.startNodes.Count).Where((int index) => ((this.startNodes[index] == node1 && this.endNodes[index] == node2) || (this.startNodes[index] == node2 && this.endNodes[index] == node1))).ToList();
                        double offset = -(5.0 * currentEdgeIndices.Count / 2);
                        foreach (int n in currentEdgeIndices)
                        {
                            offsets[n] = offset;
                            offset += 5;
                        }
                    }
                }

                for (int n = 0; n < startNodes.Count; n++)
                {
                    Polygon arrow = DrawArrow(this.startNodes[n], this.endNodes[n], this.nArrows[n], offsets[n]);
                    canvas.Children.Add(arrow);
                }
                return canvas;
            }

        }

        static Ellipse DrawNode(Node node)
        {
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<Ellipse>() != null);

            Ellipse ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Colors.Silver);
            ellipse.Stroke = new SolidColorBrush(Colors.Black);
            ellipse.Height = 2 * node.Radius;
            ellipse.Width = 2 * node.Radius;
            Canvas.SetLeft(ellipse, node.Position.Y - node.Radius);
            Canvas.SetTop(ellipse, node.Position.X - node.Radius);
            return ellipse;
        }

        static TextBlock DrawLabel(Node node)
        {
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<TextBlock>() != null);

            TextBlock label = new TextBlock();
            label.Text = node.Label;
            label.Measure(new Size(Double.MaxValue, Double.MaxValue));
            Canvas.SetLeft(label, node.Position.Y - (label.DesiredSize.Height / 2));
            Canvas.SetTop(label, node.Position.X - (label.DesiredSize.Width / 2));
            return label;
        }

        static Polygon DrawArrow(Node startNode, Node endNode, int nArrows, double offset)
        {
            Contract.Requires(startNode != null);
            Contract.Requires(endNode != null);
            Contract.Ensures(Contract.Result<Polygon>() != null);

            Polygon arrow = new Polygon();
            arrow.Fill = new SolidColorBrush(Colors.Black);
            arrow.Stroke = new SolidColorBrush(Colors.Black);
            arrow.StrokeMiterLimit = 0;

            Vector r = endNode.Position - startNode.Position;
            r.Normalize();
            Vector n = new Vector(-r.Y, r.X);
            Vector A = startNode.Position + startNode.Radius * r + offset * n;
            Vector B = endNode.Position - endNode.Radius * r + offset * n;

            arrow.Points.Add(new Point(A.Y, A.X));
            arrow.Points.Add(new Point(B.Y, B.X));

            if (nArrows == 0)
            {
                return arrow;
            }

            Vector C = A + ((B - A).Length - 5) * r;
            Vector D = C + 5 * n;
            Vector E = C - 5 * n;
            arrow.Points.Add(new Point(D.Y, D.X));
            arrow.Points.Add(new Point(E.Y, E.X));
            arrow.Points.Add(new Point(B.Y, B.X));

            if (nArrows == 1)
            {
                return arrow;
            }

            Vector F = B - ((B - A).Length - 5) * r;
            Vector G = F + 5 * n;
            Vector H = F - 5 * n;
            arrow.Points.Add(new Point(F.Y, F.X));
            arrow.Points.Add(new Point(G.Y, G.X));
            arrow.Points.Add(new Point(A.Y, A.X));
            arrow.Points.Add(new Point(H.Y, H.X));
            arrow.Points.Add(new Point(F.Y, F.X));

            return arrow;
        }
      

        public static void Draw(string filename)
        {
            Contract.Requires<ApplicationException>(filename != null && filename.Trim() != "", "!Please enter a filename!\n");
            Contract.Requires<ApplicationException>(Program.isValidated == true, "The model needs to be validated before drawing\n");
            Contract.Requires<ApplicationException>(Program.nodes.Count > 0, "The model is empty\n");

            List<Node> nodes = new List<Node>();
            Graph graph = new Graph(80);

            foreach (Model.Node modelNode in Program.nodes)
            {            
                Node node1 = new Node(20, modelNode.Id);
                graph.AddNode(node1);

                foreach (Model.EndTrack endTrack in modelNode.EndTracks)
                {
                    Node node2 = new Node(0, null);
                    graph.AddNode(node2);
                    graph.Link(node1, node2, 0);
                }
            }

            foreach (Model.Track track in Program.tracks)
            {
                if (track is Model.UniTrack)
                {
                    string id1 = ((Model.UniTrack)track).StartNode.Id;
                    string id2 = ((Model.UniTrack)track).EndNode.Id;
                    Node node1 = graph.Nodes.Single((Node node) => (node.Label != null) && node.Label.Equals(id1));
                    Node node2 = graph.Nodes.Single((Node node) => (node.Label != null) && node.Label.Equals(id2));
                    graph.Link(node1, node2, 1);
                }
                if (track is Model.BiTrack)
                {
                    string id1 = ((Model.BiTrack)track).FirstNode.Id;
                    string id2 = ((Model.BiTrack)track).SecondNode.Id;
                    Node node1 = graph.Nodes.Single((Node node) => (node.Label != null) && node.Label.Equals(id1));
                    Node node2 = graph.Nodes.Single((Node node) => (node.Label != null) && node.Label.Equals(id2));
                    graph.Link(node1, node2, 2);
                }
            }

            graph.Initiate();
            PhysicsSteadyState(graph.Nodes, 80, 0.01, 0.1, 0.01, 0.1);

            Canvas canvas = graph.Draw();
            string xaml = XamlWriter.Save(canvas);
            File.WriteAllText(filename, xaml);
        }

        static double PhysicsIter(List<Node> nodes, double refDistance, double friction, double repulsion, double stiffness)
        {
            Contract.Requires(nodes != null);
            Contract.Requires(nodes.All((Node node) => node != null));
            Contract.Requires(refDistance >= 0);
            Contract.Requires(friction >= 0);
            Contract.Requires(repulsion >= 0);
            Contract.Requires(stiffness >= 0);
            Contract.Ensures(Contract.Result<double>() >= 0);

            Hashtable totalForces = new Hashtable();
            foreach (Node node1 in nodes)
            {
                totalForces[node1] = new Vector(0, 0);

                Vector force = new Vector(0, 0);
                Vector r = new Vector(0, 0);
                double l = 0;
                foreach (Node neighbour in node1.Neighbours)
                {
                    r = neighbour.Position - node1.Position;
                    l = r.Length;
                    r.Normalize();

                    force = stiffness * (l - refDistance) * r;
                    totalForces[node1] = (Vector)totalForces[node1] + force;
                }

                foreach (Node node2 in nodes)
                {
                    force = new Vector(0, 0);
                    if (node2 != node1)
                    {
                        r = node2.Position - node1.Position;
                        l = r.Length;
                        r.Normalize();

                        force = -repulsion * (1 / Math.Sqrt(l + 1)) * r;

                    }
                    totalForces[node1] = (Vector)totalForces[node1] + force;
                }

                totalForces[node1] = (Vector)totalForces[node1] - friction * node1.Speed;
            }

            nodes.ForEach((Node node) => node.Move((Vector)totalForces[node]));
            return nodes.Sum((Node node) => node.Speed.Length);
        }

        static void PhysicsSteadyState(List<Node> nodes, double refDistance, double friction, double repulsion, double stiffness, double nullSpeed)
        {
            Contract.Requires(nodes != null);
            Contract.Requires(nodes.All((Node node) => node != null));
            Contract.Requires(refDistance >= 0);
            Contract.Requires(friction >= 0);
            Contract.Requires(repulsion >= 0);
            Contract.Requires(stiffness >= 0);
            Contract.Requires(nullSpeed > 0);
            Contract.Ensures(PhysicsIter(nodes, refDistance, friction, repulsion, stiffness) <= nullSpeed);

            while (PhysicsIter(nodes, refDistance, friction, repulsion, stiffness) > nullSpeed)
            {           
            }

            if (PhysicsIter(nodes, refDistance, friction, repulsion, stiffness) > nullSpeed)
            {
                PhysicsSteadyState(nodes, refDistance, friction, repulsion, stiffness, nullSpeed);
            }
        }
    }
}
