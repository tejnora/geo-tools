#ifndef all_tran_projective_planar_h_
#define all_tran_projective_planar_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class projective_planar_inner : public projective_inner 
{
public:
projective_planar_inner(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_inner(XXi,xxi,v)  
        {tt="projective_planar_inner";}

projective_planar_inner(const Vec<> hh, const Vec<> v) : projective_inner(hh,v)  
        {tt="projective_planar_inner";}

projective_planar_inner() : projective_inner()  
        {tt="projective_planar_inner";}

virtual ~projective_planar_inner (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

void ApproxSolution() 
{	
	for(int i = 1;i<=r1;i++)	if( (*xi)(i,3) != 0.0 )	throw at_exception("projective_inner_plannar::ApproxSolution","Thera are not only plannar points in the x-matrix");
	
	if(r1 < 4)	throw at_exception("projective_inner_plannar::ApproxSolution","Not enough points for approximate solution");
	int nn = 9+r1-1;
	Vec<> hh(nn),kk(r1*3);
	Mat<> JJ(r1*3,nn);
	kk.set_zero();
	JJ.set_zero();
	const double konst = 1;//-1
	//fill matrixes
	for(int i=1; i<=r1; i++)		
	{
		JJ( (i-1)*3+1,1 ) = (*xi)(i,1);
		JJ( (i-1)*3+1,2 ) = (*xi)(i,2);
		
		JJ( (i-1)*3+2,3 ) = (*xi)(i,1);
		JJ( (i-1)*3+2,4 ) = (*xi)(i,2);
				
		JJ( (i-1)*3+3,5 ) = (*xi)(i,1);
		JJ( (i-1)*3+3,6 ) = (*xi)(i,2);
		
		JJ( (i-1)*3+1,7 ) = -1;
		JJ( (i-1)*3+2,8 ) = -1;
		JJ( (i-1)*3+3,9 ) = -1;

		JJ( (i-1)*3+1,8+i ) = -((*Xi)(i,1)-x0);
		JJ( (i-1)*3+2,8+i ) = -((*Xi)(i,2)-y0);
		JJ( (i-1)*3+3,8+i ) = f;
	}
	//nutno opravit prvni bod
	JJ(1,9) = 0;
	JJ(2,9) = 0;
	JJ(3,9) = -1;

	kk(1) = konst*((*Xi)(1,1)-x0);
	kk(2) = konst*((*Xi)(1,2)-y0);
	kk(3) = -konst*f;
	
	
	//adjustment
	Mat<> JTJ,JTJi;        		
	try
	{
		JTJ=trans(JJ)*JJ;
		JTJi = inv(JTJ);		
		for(int i = 1;i <= nn;i++)	if(JTJi(i,i)<0)	throw at_exception("projective_inner_plannar::ApproxSolution","The matrix is nearly singular, pseudoinverse is used instead");			
	}		
	catch(GNU_gama::Exception::matvec& exc)
	{
		kout<<endl<<"GNU_gama exception: "<<exc.error<<"  "<<exc.description<<endl;
		JTJ=trans(JJ)*JJ;
		JTJi=pinv(JTJ);					
	}
	catch(at_exception& e)
	{
		kout<<endl<<endl<<"Alltran exception was called from (class::method): "<<e.location;
		kout<<endl<<"The desctription of exception: "<<e.description<<endl;
		JTJ=trans(JJ)*JJ;
		JTJi=pinv(JTJ);	
	}				
	hh=JTJi*trans(JJ)*kk;
	//adjustment
	
	/*
	//adjustment
	Mat<> Ji;        		
	try
	{		
		Ji = inv(JJ);			
	}		
	catch(GNU_gama::Exception::matvec& exc)
	{
		kout<<endl<<"GNU_gama exception: "<<exc.error<<"  "<<exc.description<<endl;
		Ji=pinv(JJ);					
	}
	hh=Ji*kk;
	//adjustment
	*/
		
	//arrangement		
	double norm1 = sqrt(hh(1)*hh(1)+hh(3)*hh(3)+hh(5)*hh(5));
	double norm2 = sqrt(hh(2)*hh(2)+hh(4)*hh(4)+hh(6)*hh(6));
	double norm = (norm1+norm2)/2;

	//kout<<endl<<norm1<<' '<<norm2;

	h(1)=hh(1)/norm1;h(2)=hh(2)/norm2;
	h(4)=hh(3)/norm1;h(5)=hh(4)/norm2;
	h(7)=hh(5)/norm1;h(8)=hh(6)/norm2;	
	h(10)=hh(7)/norm;h(11)=hh(8)/norm;h(12)=hh(9)/norm;		

	//doplnim ortogonalni system
	h(3) = h(4)*h(8) - h(7)*h(5);
	h(6) = h(7)*h(2) - h(1)*h(8);
	h(9) = h(1)*h(5) - h(4)*h(2);

	double norm3 = sqrt(h(3)*h(3)+h(6)*h(6)+h(9)*h(9));	
	h(3)=h(3)/norm3;h(6)=h(6)/norm3;h(9)=h(9)/norm3;
	
	hh = 1.0/norm*hh;

	Mat<> R(3,3);
	R(1,1)=h(1),R(1,2)=h(2),R(1,3)=h(3);
	R(2,1)=h(4),R(2,2)=h(5),R(2,3)=h(6);
	R(3,1)=h(7),R(3,2)=h(8),R(3,3)=h(9);

	h(10)=R(1,1)*hh(7)+R(2,1)*hh(8)+R(3,1)*hh(9);
	h(11)=R(1,2)*hh(7)+R(2,2)*hh(8)+R(3,2)*hh(9);
	h(12)=R(1,3)*hh(7)+R(2,3)*hh(8)+R(3,3)*hh(9);	

	h(13) = x0;
	h(14) = y0;
	h(15) = f;	

	kout<<'\n'<<"Vypoctene priblizky v poradi: r11, r12, ..., r32, r33, X0, Y0, Z0, x0, y0, f"<<'\n';
	for(int i = 1; i<=15; i++) kout<<h(i)<<"  ";
	kout<<endl;
}		

//void fill_matrixes() {}   -define in projective_inner
//void transform_points(){}	-define in projective_inner			
};

class projective_planar_x0y0 : public projective_planar_inner
{
public:
projective_planar_x0y0(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_planar_inner(XXi,xxi,v)  
        {tt="projective_planar_x0y0"; q=7;}

projective_planar_x0y0(const Vec<> hh, const Vec<> v) : projective_planar_inner(hh,v)  
        {tt="projective_planar_x0y0"; q=7;}

projective_planar_x0y0() : projective_planar_inner()  
        {tt="projective_planar_x0y0"; q=7;}

virtual ~projective_planar_x0y0 (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_inner
void fill_matrixes_projective_planar()
{	
	//constraints
	b(7) = - h(15) + f;	
	B(15,7) = 1;	
}

void fill_matrixes() 
{
	fill_matrixes_projective_inner();
	fill_matrixes_projective_planar();
}

//void transform_points(){}	-define in projective_inner			
};

class projective_planar : public projective_planar_inner
{
public:
projective_planar(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_planar_inner(XXi,xxi,v)  
        {tt="projective_planar"; q=9;}

projective_planar(const Vec<> hh, const Vec<> v) : projective_planar_inner(hh,v)  
        {tt="projective_planar"; q=9;}

projective_planar() : projective_planar_inner()  
        {tt="projective_planar"; q=9;}

virtual ~projective_planar (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_plannar_inner
void fill_matrixes_projective_planar()
{	
	//constraints
	b(7) = - h(13) + x0;
	b(8) = - h(14) + y0;
	b(9) = - h(15) + f;	
	B(13,7) = 1;
	B(14,8) = 1;
	B(15,9) = 1;
}

void fill_matrixes() 
{
	fill_matrixes_projective_inner();
	fill_matrixes_projective_planar();
}

//void transform_points(){}	-define in projective_inner			
};

}   // namespace spat_fig
#endif