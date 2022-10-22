#ifndef all_tran_projective_something_h_
#define all_tran_projective_something_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

class projective_x0y0_rd : public all_tran_base  
{
public:
projective_x0y0_rd(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,17)  
        {tt="projective_x0y0_rd"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_x0y0_rd(const Vec<> hh, const Vec<> v) : all_tran_base(hh,17)  
        {tt="projective_x0y0_rd"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_x0y0_rd() : all_tran_base(17)  
        {tt="projective_x0y0_rd"; c = 3; q=6;}

//for the case that projective_x0y0_rd is base function
projective_x0y0_rd(const Vec<> hh, const Vec<> v, int pp) : all_tran_base(hh,pp)  
{
	x0=v(1); y0=v(2); f=v(3); c = 3; q=6;
}        
projective_x0y0_rd(int pp) : all_tran_base(pp)	
{
	c = 3; q=6;
}
//for the case that projective_x0y0_rd is base function

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2); f=v(3); 
			all_tran_base::reset(XXi,xxi);
		}

void reset(const Vec<> hh,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2); f=v(3);
			all_tran_base::reset(hh);
		}

virtual ~projective_x0y0_rd (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = x0 - f*(r11*(X-X0)+r12*(Y-Y0)+r13*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (x-x0)*(k1*r^2+k2*r^4+k3*r^6)";
		out<<endl<<"y = y0 - f*(r21*(X-X0)+r22*(Y-Y0)+r23*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (y-y0)*(k1*r^2+k2*r^4+k3*r^6)";
		out<<endl<<"Computed parameters are (in order r11, r12, ..., r32, r33, X0, Y0, Z0, x0, y0, k1, k2, k3)";
        return out; 
	}

protected:
double x0,y0,f; //x02, y02 - distortion center

void ApproxSolution() 
{
	h.set_zero();
	Vec<> vv(3);
	vv(1)=x0; vv(2)=y0; vv(3)=f;
	projective_x0y0 at((*Xi),(*xi),vv);	//projective_x0y0 at((*Xi),(*xi),vv);
	at.solve();
	Vec<> hh = at.get_solution();			
	for(int i = 1; i <= 14; i++)	h(i) = hh(i);	
}

void fill_matrixes_projective_x0y0_rd()
{	
	//J a X0T	
	double r11=h(1),r12=h(2),r13=h(3);
	double r21=h(4),r22=h(5),r23=h(6);
	double r31=h(7),r32=h(8),r33=h(9);
	double X0=h(10),Y0=h(11),Z0=h(12);
	
	double A,B,C,r,rr,xx,yy;
	for(int i = 1; i <= r1; i++)
	{			
		A = r11*((*xi)(i,1)-X0)+r12*((*xi)(i,2)-Y0)+r13*((*xi)(i,3)-Z0);
		B = r21*((*xi)(i,1)-X0)+r22*((*xi)(i,2)-Y0)+r23*((*xi)(i,3)-Z0);
		C = r31*((*xi)(i,1)-X0)+r32*((*xi)(i,2)-Y0)+r33*((*xi)(i,3)-Z0);
		xx = (*Xi)(i,1)-h(13);
		yy = (*Xi)(i,2)-h(14);
		r = sqrt( xx*xx + yy*yy );	
		rr = h(15)*r*r + h(16)*r*r*r*r + h(17)*r*r*r*r*r*r;
		X0T(i,1) = h(13) - f*A/C - rr * xx;
		X0T(i,2) = h(14) - f*B/C - rr * yy; 
		
		J((i-1)*n+1,1) = -f*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+1,2) = -f*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+1,3) = -f*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+1,7) = f*A/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+1,8) = f*A/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+1,9) = f*A/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+1,10) = f*(r11*C-r31*A)/C/C;
		J((i-1)*n+1,11) = f*(r12*C-r32*A)/C/C; 		
		J((i-1)*n+1,12) = f*(r13*C-r33*A)/C/C; 		
		J((i-1)*n+1,13) = 1+rr+2*xx*xx*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+1,14) = 2*xx*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+1,15) = -xx*r*r;	
		J((i-1)*n+1,16) = -xx*r*r*r*r;
		J((i-1)*n+1,17) = -xx*r*r*r*r*r*r; 

		J((i-1)*n+2,4) = -f*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+2,5) = -f*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+2,6) = -f*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+2,7) = f*B/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+2,8) = f*B/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+2,9) = f*B/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+2,10) = f*(r21*C-r31*B)/C/C;
		J((i-1)*n+2,11) = f*(r22*C-r32*B)/C/C; 
		J((i-1)*n+2,12) = f*(r23*C-r33*B)/C/C;
		J((i-1)*n+2,13) = 2*xx*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+2,14) = 1+rr+2*yy*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+2,15) = -yy*r*r;	
		J((i-1)*n+2,16) = -yy*r*r*r*r;
		J((i-1)*n+2,17) = -yy*r*r*r*r*r*r;		
	}

	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(1) = - r11*r11 - r12*r12 - r13*r13 + 1;
	b(2) = - r21*r21 - r22*r22 - r23*r23 + 1;
	b(3) = - r31*r31 - r32*r32 - r33*r33 + 1;	
	all_tran_base::B(1,1) = 2*r11;
	all_tran_base::B(2,1) = 2*r12;
	all_tran_base::B(3,1) = 2*r13;
	all_tran_base::B(4,2) = 2*r21;
	all_tran_base::B(5,2) = 2*r22;
	all_tran_base::B(6,2) = 2*r23;
	all_tran_base::B(7,3) = 2*r31;
	all_tran_base::B(8,3) = 2*r32;
	all_tran_base::B(9,3) = 2*r33;

	b(4) = - r11*r21 - r12*r22 - r13*r23;
	b(5) = - r21*r31 - r22*r32 - r23*r33;
	b(6) = - r11*r31 - r12*r32 - r13*r33;	
	//J
	all_tran_base::B(1,4) = r21;
	all_tran_base::B(2,4) = r22;
	all_tran_base::B(3,4) = r23;
	all_tran_base::B(4,4) = r11;
	all_tran_base::B(5,4) = r12;
	all_tran_base::B(6,4) = r13;
	
	all_tran_base::B(4,5) = r31;
	all_tran_base::B(5,5) = r32;
	all_tran_base::B(6,5) = r33;
	all_tran_base::B(7,5) = r21;
	all_tran_base::B(8,5) = r22;
	all_tran_base::B(9,5) = r23;

	all_tran_base::B(1,6) = r31;
	all_tran_base::B(2,6) = r32;
	all_tran_base::B(3,6) = r33;
	all_tran_base::B(7,6) = r11;
	all_tran_base::B(8,6) = r12;
	all_tran_base::B(9,6) = r13;
}

void fill_matrixes()
{
	fill_matrixes_projective_x0y0_rd();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4); //point numbers
		
		double r11=h(1),r12=h(2),r13=h(3);
		double r21=h(4),r22=h(5),r23=h(6);
		double r31=h(7),r32=h(8),r33=h(9);
		double X0=h(10),Y0=h(11),Z0=h(12);
		double A,B,C,r,rr,xx,yy;
		
		for(int i = 1; i <= num; i++)
		{
			A = r11*(x(i,1)-X0)+r12*(x(i,2)-Y0)+r13*(x(i,3)-Z0);
			B = r21*(x(i,1)-X0)+r22*(x(i,2)-Y0)+r23*(x(i,3)-Z0);
			C = r31*(x(i,1)-X0)+r32*(x(i,2)-Y0)+r33*(x(i,3)-Z0);			
			X(i,1) = h(13) - f*A/C;
			X(i,2) = h(14) - f*B/C; 			
			for(int j = 1; j <= 4; j++)
			{
				xx = X(i,1)-h(13);
				yy = X(i,2)-h(14);
				r = sqrt( xx*xx + yy*yy );	
				rr = h(15)*r*r + h(16)*r*r*r*r + h(17)*r*r*r*r*r*r;								
				X(i,1) = h(13) - f*A/C - rr * xx;
				X(i,2) = h(14) - f*B/C - rr * yy;				
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("projective_x0y0_rd::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};


class projective_x0y0_rd_td : public projective_x0y0_rd  
{
public:
projective_x0y0_rd_td(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_x0y0_rd(XXi,xxi,v)  
        {tt="projective_x0y0_rd_td"; all_tran_base::reset(19);}

projective_x0y0_rd_td(const Vec<> hh, const Vec<> v) : projective_x0y0_rd(hh,v,19)  
        {tt="projective_x0y0_rd_td"; }

projective_x0y0_rd_td() : projective_x0y0_rd(19)  
        {tt="projective_x0y0_rd_td"; }

virtual ~projective_x0y0_rd_td (){}

ostream& report_constants(ostream& out) const    //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = x0 - f*(r11*(X-X0)+r12*(Y-Y0)+r13*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (x-x0)*(k1*r^2+k2*r^4+k3*r^6) - p1*(r^2+2*(x-x0)^2) - 2*p2*(x-x0)*(y-y0)";
		out<<endl<<"y = y0 - f*(r21*(X-X0)+r22*(Y-Y0)+r23*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (y-y0)*(k1*r^2+k2*r^4+k3*r^6) - p2*(r^2+2*(y-y0)^2) - 2*p1*(x-x0)*(y-y0)";
		out<<endl<<"Computed parameters are (in order r11, r12, ..., r32, r33, X0, Y0, Z0, x0, y0, k1, k2, k3, p1, p2)";
        return out; 
	}

protected:

void ApproxSolution() 
{
	h.set_zero();
	Vec<> vv(3);
	vv(1)=x0; vv(2)=y0; vv(3)=f;
	projective_x0y0_rd at((*Xi),(*xi),vv);	//projective_x0y0 at((*Xi),(*xi),vv);
	at.solve();
	Vec<> hh = at.get_solution();			
	for(int i = 1; i <= 17; i++)	h(i) = hh(i);	
}

void fill_matrixes_projective_x0y0_rd_td()
{			
	double r,xx,yy;
	for(int i = 1; i <= r1; i++)
	{			
		xx = (*Xi)(i,1)-h(13);
		yy = (*Xi)(i,2)-h(14);
		r = sqrt( xx*xx + yy*yy );	

		X0T(i,1) += - h(18)*(r*r+2*xx*xx) - 2*h(19)*xx*yy;;
		X0T(i,2) += - h(19)*(r*r+2*yy*yy) - 2*h(18)*xx*yy; 
				
		J((i-1)*n+1,13) += 6*h(18)*xx + 2*h(19)*yy;
		J((i-1)*n+1,14) += 2*h(18)*yy + 2*h(19)*xx;
		J((i-1)*n+1,18) = -r*r-2*xx*xx; //- p1*(r^2+2*(x-x0)^2) - 2*p2*(x-x0)*(y-y0)";
		J((i-1)*n+1,19) = -2*xx*yy;

		J((i-1)*n+2,13) += 2*h(18)*yy + 2*h(19)*xx;
		J((i-1)*n+2,14) += 6*h(19)*yy + 2*h(19)*xx;
		J((i-1)*n+2,18) = -2*xx*yy;	//- p2*(r^2+2*(y-y0)^2) - 2*p1*(x-x0)*(y-y0)";
		J((i-1)*n+2,19) = -r*r-2*yy*yy;			
	}	
}

void fill_matrixes()
{
	fill_matrixes_projective_x0y0_rd();
	fill_matrixes_projective_x0y0_rd_td();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4); //point numbers
		
		double r11=h(1),r12=h(2),r13=h(3);
		double r21=h(4),r22=h(5),r23=h(6);
		double r31=h(7),r32=h(8),r33=h(9);
		double X0=h(10),Y0=h(11),Z0=h(12);
		double A,B,C,r,rr,xx,yy;
		
		for(int i = 1; i <= num; i++)
		{
			A = r11*(x(i,1)-X0)+r12*(x(i,2)-Y0)+r13*(x(i,3)-Z0);
			B = r21*(x(i,1)-X0)+r22*(x(i,2)-Y0)+r23*(x(i,3)-Z0);
			C = r31*(x(i,1)-X0)+r32*(x(i,2)-Y0)+r33*(x(i,3)-Z0);		
			X(i,1) = h(13) - f*A/C;
			X(i,2) = h(14) - f*B/C; 			
			for(int j = 1; j <= 4; j++)
			{
				xx = X(i,1)-h(13);	//- p1*(r^2+2*(x-x0)^2) - 2*p2*(x-x0)*(y-y0)";	
				yy = X(i,2)-h(14);	//- p2*(r^2+2*(y-y0)^2) - 2*p1*(x-x0)*(y-y0)";
				r = sqrt( xx*xx + yy*yy );	
				rr = h(15)*r*r + h(16)*r*r*r*r + h(17)*r*r*r*r*r*r;								
				X(i,1) = h(13) - f*A/C - rr * xx - h(18)*(r*r+2*xx*xx) - 2*h(19)*xx*yy;
				X(i,2) = h(14) - f*B/C - rr * yy - h(19)*(r*r+2*yy*yy) - 2*h(18)*xx*yy;				
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("projective_x0y0_rd_td::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};


class projective_rd2 : public all_tran_base  
{
public:
projective_rd2(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,17)  
        {tt="projective_rd2"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_rd2(const Vec<> hh, const Vec<> v) : all_tran_base(hh,17)  
        {tt="projective_rd2"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_rd2() : all_tran_base(17)  
        {tt="projective_rd2"; c = 3; q=6;}

//for the case that projective_rd2 is base function
projective_rd2(const Vec<> hh, const Vec<> v, int pp) : all_tran_base(hh,pp)  
{
	x0=v(1); y0=v(2); f=v(3); c = 3; q=6;
}        
projective_rd2(int pp) : all_tran_base(pp)	
{
	c = 3; q=6;
}
//for the case that projective_rd2 is base function

void reset(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2); f=v(3); 
			all_tran_base::reset(XXi,xxi);
		}

void reset(const Vec<> hh,const Vec<> v)  //virtual
		{
			x0=v(1); y0=v(2); f=v(3);
			all_tran_base::reset(hh);
		}

virtual ~projective_rd2 (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = x0 - f*(r11*(X-X0)+r12*(Y-Y0)+r13*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (x-x02)*(k1*r^2+k2*r^4+k3*r^6)";
		out<<endl<<"y = y0 - f*(r21*(X-X0)+r22*(Y-Y0)+r23*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0)) - (y-y02)*(k1*r^2+k2*r^4+k3*r^6)";
		out<<endl<<"Computed parameters are (in order r11, r12, ..., r32, r33, X0, Y0, Z0, x02, y02, k1, k2, k3)";
        return out; 
	}

protected:
double x0,y0,f; //x02, y02 - distortion center

void ApproxSolution() 
{
	h.set_zero();
	Vec<> vv(3);
	vv(1)=x0; vv(2)=y0; vv(3)=f;
	projective at((*Xi),(*xi),vv);	//projective_x0y0 at((*Xi),(*xi),vv);
	at.solve();
	Vec<> hh = at.get_solution();			
	for(int i = 1; i <= 14; i++)	h(i) = hh(i);	
}

void fill_matrixes_projective_x0y0_rd()
{	
	//J a X0T	
	double r11=h(1),r12=h(2),r13=h(3);
	double r21=h(4),r22=h(5),r23=h(6);
	double r31=h(7),r32=h(8),r33=h(9);
	double X0=h(10),Y0=h(11),Z0=h(12);
	
	double A,B,C,r,rr,xx,yy;
	for(int i = 1; i <= r1; i++)
	{			
		A = r11*((*xi)(i,1)-X0)+r12*((*xi)(i,2)-Y0)+r13*((*xi)(i,3)-Z0);
		B = r21*((*xi)(i,1)-X0)+r22*((*xi)(i,2)-Y0)+r23*((*xi)(i,3)-Z0);
		C = r31*((*xi)(i,1)-X0)+r32*((*xi)(i,2)-Y0)+r33*((*xi)(i,3)-Z0);
		xx = (*Xi)(i,1)-h(13);
		yy = (*Xi)(i,2)-h(14);
		r = sqrt( xx*xx + yy*yy );	
		rr = h(15)*r*r + h(16)*r*r*r*r + h(17)*r*r*r*r*r*r;
		X0T(i,1) = x0 - f*A/C - rr * xx;
		X0T(i,2) = y0 - f*B/C - rr * yy; 
		
		J((i-1)*n+1,1) = -f*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+1,2) = -f*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+1,3) = -f*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+1,7) = f*A/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+1,8) = f*A/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+1,9) = f*A/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+1,10) = f*(r11*C-r31*A)/C/C;
		J((i-1)*n+1,11) = f*(r12*C-r32*A)/C/C; 		
		J((i-1)*n+1,12) = f*(r13*C-r33*A)/C/C; 		
		J((i-1)*n+1,13) = rr+2*xx*xx*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+1,14) = 2*xx*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+1,15) = -xx*r*r;	
		J((i-1)*n+1,16) = -xx*r*r*r*r;
		J((i-1)*n+1,17) = -xx*r*r*r*r*r*r; 

		J((i-1)*n+2,4) = -f*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+2,5) = -f*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+2,6) = -f*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+2,7) = f*B/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+2,8) = f*B/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+2,9) = f*B/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+2,10) = f*(r21*C-r31*B)/C/C;
		J((i-1)*n+2,11) = f*(r22*C-r32*B)/C/C; 
		J((i-1)*n+2,12) = f*(r23*C-r33*B)/C/C;
		J((i-1)*n+2,13) = 2*xx*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+2,14) = rr+2*yy*yy*(h(15)+2*h(16)*r*r+3*h(17)*r*r*r*r);
		J((i-1)*n+2,15) = -yy*r*r;	
		J((i-1)*n+2,16) = -yy*r*r*r*r;
		J((i-1)*n+2,17) = -yy*r*r*r*r*r*r;		
	}

	//constraints
	//dX directly instead of X0T, dX = -X0T		
	b(1) = - r11*r11 - r12*r12 - r13*r13 + 1;
	b(2) = - r21*r21 - r22*r22 - r23*r23 + 1;
	b(3) = - r31*r31 - r32*r32 - r33*r33 + 1;	
	all_tran_base::B(1,1) = 2*r11;
	all_tran_base::B(2,1) = 2*r12;
	all_tran_base::B(3,1) = 2*r13;
	all_tran_base::B(4,2) = 2*r21;
	all_tran_base::B(5,2) = 2*r22;
	all_tran_base::B(6,2) = 2*r23;
	all_tran_base::B(7,3) = 2*r31;
	all_tran_base::B(8,3) = 2*r32;
	all_tran_base::B(9,3) = 2*r33;

	b(4) = - r11*r21 - r12*r22 - r13*r23;
	b(5) = - r21*r31 - r22*r32 - r23*r33;
	b(6) = - r11*r31 - r12*r32 - r13*r33;	
	//J
	all_tran_base::B(1,4) = r21;
	all_tran_base::B(2,4) = r22;
	all_tran_base::B(3,4) = r23;
	all_tran_base::B(4,4) = r11;
	all_tran_base::B(5,4) = r12;
	all_tran_base::B(6,4) = r13;
	
	all_tran_base::B(4,5) = r31;
	all_tran_base::B(5,5) = r32;
	all_tran_base::B(6,5) = r33;
	all_tran_base::B(7,5) = r21;
	all_tran_base::B(8,5) = r22;
	all_tran_base::B(9,5) = r23;

	all_tran_base::B(1,6) = r31;
	all_tran_base::B(2,6) = r32;
	all_tran_base::B(3,6) = r33;
	all_tran_base::B(7,6) = r11;
	all_tran_base::B(8,6) = r12;
	all_tran_base::B(9,6) = r13;
}

void fill_matrixes()
{
	fill_matrixes_projective_x0y0_rd();
}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		int num = x.rows();
		Mat<> X(num,3);	
		for(int j = 1;j<=num;j++)	X(j,3) = x(j,4); //point numbers
		
		double r11=h(1),r12=h(2),r13=h(3);
		double r21=h(4),r22=h(5),r23=h(6);
		double r31=h(7),r32=h(8),r33=h(9);
		double X0=h(10),Y0=h(11),Z0=h(12);
		double A,B,C,r,rr,xx,yy;
		
		for(int i = 1; i <= num; i++)
		{
			A = r11*(x(i,1)-X0)+r12*(x(i,2)-Y0)+r13*(x(i,3)-Z0);
			B = r21*(x(i,1)-X0)+r22*(x(i,2)-Y0)+r23*(x(i,3)-Z0);
			C = r31*(x(i,1)-X0)+r32*(x(i,2)-Y0)+r33*(x(i,3)-Z0);			
			X(i,1) = x0 - f*A/C;
			X(i,2) = y0 - f*B/C; 			
			for(int j = 1; j <= 4; j++)
			{
				xx = X(i,1)-h(13);
				yy = X(i,2)-h(14);
				r = sqrt( xx*xx + yy*yy );	
				rr = h(15)*r*r + h(16)*r*r*r*r + h(17)*r*r*r*r*r*r;								
				X(i,1) = x0 - f*A/C - rr * xx;
				X(i,2) = y0 - f*B/C - rr * yy;				
			}
		}	
		return X;
	}	
	else
	{
		throw at_exception("projective_rd2::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};


}   // namespace all_tran
#endif