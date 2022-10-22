#ifndef all_tran_projective_double_h_
#define all_tran_projective_double_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class projective_double_inner : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
projective_double_inner(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,27)  
        {tt="projective_double_inner"; x0=v(1); y0=v(2); f=v(3); c = 3; q=12;}

projective_double_inner(const Vec<> hh, const Vec<> v) : all_tran_base(hh,27)  
        {tt="projective_double_inner"; x0=v(1); y0=v(2); f=v(3); c = 3; q=12;}

projective_double_inner() : all_tran_base(27)  
        {tt="projective_double_inner"; c = 3; q=12;}

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

virtual ~projective_double_inner (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x_i = x0 - f*(r11_i*(X-X0_i)+r12_i*(Y-Y0_i)+r13_i*(Z-Z0_i))/(r31_i*(X-X0_i)+r32_i*(Y-Y0_i)+r33_i*(Z-Z0_i))";
		out<<endl<<"y_i = y0 - f*(r21_i*(X-X0_i)+r22_i*(Y-Y0_i)+r23_i*(Z-Z0_i))/(r31_i*(X-X0_i)+r32_i*(Y-Y0_i)+r33_i*(Z-Z0_i))";
        out<<endl<<"Computed parameters are (in order r11_1, r12_1, ..., r33_1, X0_1, Y0_1, Z0_1, r11_n, r12_n, ..., Z0_n, x0, y0, f)";
        return out;
	}

protected:
double x0,y0,f;

void ApproxSolution() 
{		
	h.set_zero();
	Vec<> vv(3);
	vv(1)=x0; vv(2)=y0; vv(3)=f;
	int dim = r1/2;
	Mat<> Xi1(dim,3);
	Mat<> Xi2(dim,3);
	for(int i = 1;i<=dim;i++)
	{		
		Xi1(i,1) = (*Xi)(i,1);
		Xi1(i,2) = (*Xi)(i,2);
		Xi1(i,3) = (*Xi)(i,3);
		Xi2(i,1) = (*Xi)(dim+i,1);
		Xi2(i,2) = (*Xi)(dim+i,2);
		Xi2(i,3) = (*Xi)(dim+i,3);
	}
	
	//kout<<(*Xi)<<endl<<(*xi)<<endl;
	//kout<<Xi1<<endl<<Xi2<<endl;

	projective_planar at1(Xi1,(*xi),vv);	
	at1.solve();
	Vec<> hh1 = at1.get_solution();
	
	/*
	Vec<> hh1(12);
	hh1(1)=0.9983404434; 
	hh1(2)=-0.03481767386;  
	hh1(3)=0.04697403764;  
	hh1(4)=-0.004771417973;  
	hh1(5)=0.7534175176;  
	hh1(6)=0.6574833755;  
	hh1(7)=-0.05819663146;  
	hh1(8)=-0.6565105041;  
	hh1(9)=0.752036894;  
	hh1(10)=386.274618;  
	hh1(11)=-434.9969828;  
	hh1(12)=729.6730088; 
	*/

	//kout<<Xi2<<endl<<(*xi)<<endl;

	projective_planar at2(Xi2,(*xi),vv);
	at2.solve();	
	Vec<> hh2 = at2.get_solution();
	for(int i = 1; i <= 12; i++)	h(i) = hh1(i);
	for(int i = 1; i <= 15; i++)	h(12+i) = hh2(i);
}

void fill_matrixes_projective_double_inner()
{	
	//image 1
	//J a X0T	
	double r11=h(1),r12=h(2),r13=h(3);
	double r21=h(4),r22=h(5),r23=h(6);
	double r31=h(7),r32=h(8),r33=h(9);
	double X0=h(10),Y0=h(11),Z0=h(12);
	
	double A,B,C;
	int dim = r1/2;
	for(int i = 1; i <= dim; i++)
	{			
		A = r11*((*xi)(i,1)-X0)+r12*((*xi)(i,2)-Y0)+r13*((*xi)(i,3)-Z0);
		B = r21*((*xi)(i,1)-X0)+r22*((*xi)(i,2)-Y0)+r23*((*xi)(i,3)-Z0);
		C = r31*((*xi)(i,1)-X0)+r32*((*xi)(i,2)-Y0)+r33*((*xi)(i,3)-Z0);		
		X0T(i,1) = h(25)-h(27)*A/C;
		X0T(i,2) = h(26)-h(27)*B/C; 
		
		J((i-1)*n+1,1) = -h(27)*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+1,2) = -h(27)*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+1,3) = -h(27)*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+1,7) = h(27)*A/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+1,8) = h(27)*A/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+1,9) = h(27)*A/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+1,10) = h(27)*(r11*C-r31*A)/C/C;
		J((i-1)*n+1,11) = h(27)*(r12*C-r32*A)/C/C; 
		J((i-1)*n+1,12) = h(27)*(r13*C-r33*A)/C/C; 
		J((i-1)*n+1,25) = 1;		
		J((i-1)*n+1,27) = -A/C; 

		J((i-1)*n+2,4) = -h(27)*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+2,5) = -h(27)*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+2,6) = -h(27)*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+2,7) = h(27)*B/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+2,8) = h(27)*B/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+2,9) = h(27)*B/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+2,10) = h(27)*(r21*C-r31*B)/C/C;
		J((i-1)*n+2,11) = h(27)*(r22*C-r32*B)/C/C; 
		J((i-1)*n+2,12) = h(27)*(r23*C-r33*B)/C/C;
		J((i-1)*n+2,26) = 1;		
		J((i-1)*n+2,27) = -B/C; 
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
	//image 1

	//image 2
	r11=h(13),r12=h(14),r13=h(15);
	r21=h(16),r22=h(17),r23=h(18);
	r31=h(19),r32=h(20),r33=h(21);
	X0=h(22),Y0=h(23),Z0=h(24);	
	
	for(int i = 1; i <= dim; i++)
	{			
		A = r11*((*xi)(i,1)-X0)+r12*((*xi)(i,2)-Y0)+r13*((*xi)(i,3)-Z0);
		B = r21*((*xi)(i,1)-X0)+r22*((*xi)(i,2)-Y0)+r23*((*xi)(i,3)-Z0);
		C = r31*((*xi)(i,1)-X0)+r32*((*xi)(i,2)-Y0)+r33*((*xi)(i,3)-Z0);		
		X0T(dim+i,1) = h(25)-h(27)*A/C;
		X0T(dim+i,2) = h(26)-h(27)*B/C; 
		
		J((dim+i-1)*n+1,13) = -h(27)*((*xi)(i,1)-X0)/C; 
		J((dim+i-1)*n+1,14) = -h(27)*((*xi)(i,2)-Y0)/C;
		J((dim+i-1)*n+1,15) = -h(27)*((*xi)(i,3)-Z0)/C;
		J((dim+i-1)*n+1,19) = h(27)*A/C/C*((*xi)(i,1)-X0); 
		J((dim+i-1)*n+1,20) = h(27)*A/C/C*((*xi)(i,2)-Y0); 
		J((dim+i-1)*n+1,21) = h(27)*A/C/C*((*xi)(i,3)-Z0); 
		J((dim+i-1)*n+1,22) = h(27)*(r11*C-r31*A)/C/C;
		J((dim+i-1)*n+1,23) = h(27)*(r12*C-r32*A)/C/C; 
		J((dim+i-1)*n+1,24) = h(27)*(r13*C-r33*A)/C/C; 
		J((dim+i-1)*n+1,25) = 1;		
		J((dim+i-1)*n+1,27) = -A/C; 

		J((dim+i-1)*n+2,16) = -h(27)*((*xi)(i,1)-X0)/C; 
		J((dim+i-1)*n+2,17) = -h(27)*((*xi)(i,2)-Y0)/C;
		J((dim+i-1)*n+2,18) = -h(27)*((*xi)(i,3)-Z0)/C;
		J((dim+i-1)*n+2,19) = h(27)*B/C/C*((*xi)(i,1)-X0); 
		J((dim+i-1)*n+2,20) = h(27)*B/C/C*((*xi)(i,2)-Y0); 
		J((dim+i-1)*n+2,21) = h(27)*B/C/C*((*xi)(i,3)-Z0); 
		J((dim+i-1)*n+2,22) = h(27)*(r21*C-r31*B)/C/C;
		J((dim+i-1)*n+2,23) = h(27)*(r22*C-r32*B)/C/C; 
		J((dim+i-1)*n+2,24) = h(27)*(r23*C-r33*B)/C/C;
		J((dim+i-1)*n+2,26) = 1;		
		J((dim+i-1)*n+2,27) = -B/C; 
	}

	//constraints
	//dX directly instead of X0T, dX = -X0T	
	b(7) = - r11*r11 - r12*r12 - r13*r13 + 1;
	b(8) = - r21*r21 - r22*r22 - r23*r23 + 1;
	b(9) = - r31*r31 - r32*r32 - r33*r33 + 1;	
	all_tran_base::B(13,7) = 2*r11;
	all_tran_base::B(14,7) = 2*r12;
	all_tran_base::B(15,7) = 2*r13;
	all_tran_base::B(16,8) = 2*r21;
	all_tran_base::B(17,8) = 2*r22;
	all_tran_base::B(18,8) = 2*r23;
	all_tran_base::B(19,9) = 2*r31;
	all_tran_base::B(20,9) = 2*r32;
	all_tran_base::B(21,9) = 2*r33;

	b(10) = - r11*r21 - r12*r22 - r13*r23;
	b(11) = - r21*r31 - r22*r32 - r23*r33;
	b(12) = - r11*r31 - r12*r32 - r13*r33;	
	//J
	all_tran_base::B(13,10) = r21;
	all_tran_base::B(14,10) = r22;
	all_tran_base::B(15,10) = r23;
	all_tran_base::B(16,10) = r11;
	all_tran_base::B(17,10) = r12;
	all_tran_base::B(18,10) = r13;
	
	all_tran_base::B(16,11) = r31;
	all_tran_base::B(17,11) = r32;
	all_tran_base::B(18,11) = r33;
	all_tran_base::B(19,11) = r21;
	all_tran_base::B(20,11) = r22;
	all_tran_base::B(21,11) = r23;

	all_tran_base::B(13,12) = r31;
	all_tran_base::B(14,12) = r32;
	all_tran_base::B(15,12) = r33;
	all_tran_base::B(19,12) = r11;
	all_tran_base::B(20,12) = r12;
	all_tran_base::B(21,12) = r13;
	//image 2
}

void fill_matrixes() {fill_matrixes_projective_double_inner();}

Mat<> transform_points(const Mat<>& x)
{
	throw at_exception("projective_double_inner::transform_points",	"The method \"transform_points\" is not defined");
}


/*
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
		double A,B,C;
		
		for(int i = 1; i <= num; i++)
		{
			A = r11*(x(i,1)-X0)+r12*(x(i,2)-Y0)+r13*(x(i,3)-Z0);
			B = r21*(x(i,1)-X0)+r22*(x(i,2)-Y0)+r23*(x(i,3)-Z0);
			C = r31*(x(i,1)-X0)+r32*(x(i,2)-Y0)+r33*(x(i,3)-Z0);		
			X(i,1) = h(25)-h(27)*A/C;
			X(i,2) = h(26)-h(27)*B/C; 
		}	
		return X;
	}	
	else
	{
		throw at_exception("projective_double_inner::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
*/
			
};

class projective_double : public projective_double_inner  
{
public:
projective_double(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_double_inner(XXi,xxi,v)  
        {tt="projective_double"; q=15;}

projective_double(const Vec<> hh, const Vec<> v) : projective_double_inner(hh,v)  
         {tt="projective_double"; q=15;}

projective_double() : projective_double_inner()  
        {tt="projective_double"; q=15;}

virtual ~projective_double (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_double_inner
void fill_matrixes_projective_double()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T		
	b(13) = - h(25) + x0;
	b(14) = - h(26) + y0;
	b(15) = - h(27) + f;	
	B(25,13) = 1;
	B(26,14) = 1;
	B(27,15) = 1;
}

void fill_matrixes() 
{
	fill_matrixes_projective_double_inner();
	fill_matrixes_projective_double();
}

//void transform_points(){}	-define in projective_inner			
};




}   // namespace spat_fig
#endif