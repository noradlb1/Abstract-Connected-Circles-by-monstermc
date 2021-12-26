Public Class Form1
    Private MainRect As Rectangle
    Private Shared rd As New Random
    Private Shared Circles(250) As cCircle
    Private Shared PointDistance As Decimal = 65
    Private Shared ea As MouseEventArgs

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Invalidate()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DoubleBuffered = True
        PrepareCircles()
    End Sub

    Private Sub PrepareCircles()
        'get form rectangle
        MainRect = DisplayRectangle

        'prepare circles
        For i As Integer = 0 To Circles.Count - 1
            Circles(i) = New cCircle(MainRect)
        Next
    End Sub

    Private Sub Form1_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave
        ea = Nothing
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        ea = e
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim G As Graphics = e.Graphics

        G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        For i As Integer = 0 To Circles.Count - 1

            If Not IsNothing(ea) Then
                Dim msx As Integer = ea.X
                Dim msy As Integer = ea.Y
                Dim osx As Integer = Circles(i).x
                Dim osy As Integer = Circles(i).y

                'detect the mouse location. if a circle is within the range (100px) of mouse pointer
                'then push back the circle
                If (msx - osx) ^ 2 + (msy - osy) ^ 2 < 100 ^ 2 Then
                    Dim pTarget As Point = New Point(osx, osy)
                    Dim pOrigin As Point = New Point(msx, msy)

                    'get the angle of cricle from mouse pointer location
                    Dim getAngle As Integer
                    getAngle = (((Math.Atan2(osx - msx, msy - osy) * (180 / Math.PI)) + 360.0) Mod 360.0)

                    'get the distance of circle from mouse pointer location
                    Dim getDist As Integer = DistanceBetween(New Point(msx, msy), New Point(osx, osy))

                    'get the new point where the circle should be pushed back 
                    Dim newPoint As Point = New Point(GetX(osx, 100 - getDist, getAngle), GetY(osy, 100 - getDist, getAngle))

                    'set the new point to the circle
                    Circles(i).x = newPoint.X
                    Circles(i).y = newPoint.Y
                End If
            End If

            'update circles
            Circles(i).Show(G)
            Circles(i).Update()
        Next
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        PrepareCircles()
    End Sub

    Friend Class cCircle
        Public movementAngle As Decimal
        Public speed As Decimal
        Public size As Decimal
        Public x As Decimal
        Public y As Decimal
        Private MainRect As Rectangle

        Sub New(MainRect As Rectangle)
            Me.MainRect = MainRect
            ResetVars()
        End Sub

        Private Sub ResetVars()
            'reset variables
            movementAngle = rd.Next(0, 360)
            speed = rd.Next(2, 7)
            size = rd.Next(2, 10)
            x = rd.Next(0, MainRect.Width)
            y = rd.Next(0, MainRect.Height)
        End Sub

        Public Sub Show(G As Graphics)
            Dim mypoint As Point = New Point(x, y)

            'loop to all circles to identify nearby circles 
            For i As Integer = 0 To Circles.Count - 1
                Dim cpoint As Point = New Point(Circles(i).x, Circles(i).y)

                If Circles(i).x <> x And Circles(i).y <> y Then
                    'get the distance between 2 circles
                    Dim iDis As Integer = DistanceBetween(mypoint, cpoint)
                    If iDis < PointDistance Then
                        'set the alpha of the line based on the distance
                        'fade when far and more visible when near
                        Dim a As Integer = (iDis / PointDistance) * 50
                        G.DrawLine(New Pen(Color.FromArgb(50 - a, 0, 255, 0), 0.5), mypoint, cpoint)
                    End If
                End If
            Next

            G.FillEllipse(New SolidBrush(Color.FromArgb(255, 0, 220, 0)), New Rectangle(x - (size / 2), y - (size / 2), size, size))
        End Sub

        Public Sub Update()
            'move the position of the circle based on the given speed and angle
            x = GetX(x, speed, movementAngle)
            y = GetY(y, speed, movementAngle)

            'reset variables when the circle reaches the edge
            If x < -20 Or y < -20 Or x > MainRect.Width + 20 Or y > MainRect.Height + 20 Then
                ResetVars()
            End If
        End Sub

    End Class

    Public Shared Function DistanceBetween(p1 As Point, p2 As Point) As Single
        Return Math.Sqrt((Math.Abs(p2.X - p1.X) ^ 2) + (Math.Abs(p2.Y - p1.Y) ^ 2))
    End Function

    Private Shared Function GetX(FromX As Decimal, toAdd As Decimal, Angle As Integer) As Decimal
        Return FromX + toAdd * Math.Cos(If(Angle - 90 < 0, 360 + (Angle - 90), Angle - 90) * Math.PI / 180)
    End Function

    Private Shared Function GetY(FromY As Decimal, toAdd As Decimal, Angle As Integer) As Decimal
        Return FromY + toAdd * Math.Sin(If(Angle - 90 < 0, 360 + (Angle - 90), Angle - 90) * Math.PI / 180)
    End Function

End Class
