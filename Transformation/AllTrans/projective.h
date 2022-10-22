#ifndef all_tran_projective_h_
#define all_tran_projective_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------
double distance_3d(const Mat<>& xd,int i,int j)
{
	return sqrt( (xd(i,1)-xd(j,1))*(xd(i,1)-xd(j,1)) + (xd(i,2)-xd(j,2))*(xd(i,2)-xd(j,2)) + (xd(i,3)-xd(j,3))*(xd(i,3)-xd(j,3)) );
}

double distance_2d(const Mat<>& xd,int i,int j)
{
	return sqrt( (xd(i,1)-xd(j,1))*(xd(i,1)-xd(j,1)) + (xd(i,2)-xd(j,2))*(xd(i,2)-xd(j,2)) );
}

double bearing(const Mat<>& xd,int i,int j)
{	
	if(i == j) return 0.0;
	double a = atan2(xd(j,2)-xd(i,2),xd(j,1)-xd(i,1));
	if (a < 0) a += 2.0*M_PI;
	return a;
}

class projective_inner : public all_tran_base  
{

 //in this class is changed the symbol of picture coordinates from xi to Xi and the symbol of spatial coordinates from Xi to xi

public:
projective_inner(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : all_tran_base(XXi,xxi,15)  
        {tt="projective_inner"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_inner(const Vec<> hh, const Vec<> v) : all_tran_base(hh,15)  
        {tt="projective_inner"; x0=v(1); y0=v(2); f=v(3); c = 3; q=6;}

projective_inner() : all_tran_base(15)  
        {tt="projective_inner"; c = 3; q=6;}

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

virtual ~projective_inner (){}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<endl<<endl<<tt;
		out<<endl<<"The equation of "<<tt<<" is:";
		out<<endl<<"x = x0 - f*(r11*(X-X0)+r12*(Y-Y0)+r13*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0))";
		out<<endl<<"y = y0 - f*(r21*(X-X0)+r22*(Y-Y0)+r23*(Z-Z0))/(r31*(X-X0)+r32*(Y-Y0)+r33*(Z-Z0))";
        out<<endl<<"Computed parameters are (in order r11, r12, ..., r32, r33, X0, Y0, Z0, x0, y0, f)";
        return out;
	}

protected:
double x0,y0,f;

void choose_four_suitable_points(Mat<>& Xa);
double inner_angle(Mat<>& Xa, int a, int b);

void ApproxSolution() 
{		
	if(r1 < 4)	throw at_exception("projective_inner::ApproxSolution","Not enough points for approximate solution");	
	h(13) = x0;
	h(14) = y0;
	h(15) = f;

	Mat<> Xa(4,3);
	if(r1 > 4)	choose_four_suitable_points(Xa);
	else	for(int i = 1;i<=4;i++)
				for(int j = 1;j<=3;j++)	Xa(i,j) = (*Xi)(i,j);

	Mat<> xa(4,4);
	for(int i = 1; i <= r1; i++)
	{
		if(Xa(1,3) == (*xi)(i,4))	for(int j = 1; j<=4; j++)	xa(1,j)=(*xi)(i,j);
		if(Xa(2,3) == (*xi)(i,4))	for(int j = 1; j<=4; j++)	xa(2,j)=(*xi)(i,j);
		if(Xa(3,3) == (*xi)(i,4))	for(int j = 1; j<=4; j++)	xa(3,j)=(*xi)(i,j);
		if(Xa(4,3) == (*xi)(i,4))	for(int j = 1; j<=4; j++)	xa(4,j)=(*xi)(i,j);
	}

	//v
	double a,b,c,alfa,beta,gama,A,B,C,D;
	Vec<> e(5),ef(5);
	//123	
	a = distance_3d(xa,1,2);
	b = distance_3d(xa,2,3);
	c = distance_3d(xa,1,3);
	A = (b*b+c*c)/(a*a);
	B = ( (b*b-c*c)/(a*a) * (b*b-c*c)/(a*a) );
	C = c*c/(a*a);
	D = b*b/(a*a);	
	
	alfa = inner_angle(Xa,1,2);
	beta = inner_angle(Xa,2,3);
	gama = inner_angle(Xa,1,3);

	e(1) = 1 - 2*A + B + 4*C*sin(beta)*sin(beta);
	e(2) = 4*(-cos(beta)*cos(gama)+A*(cos(beta)*cos(gama)+cos(alfa))-B*cos(alfa)
			-2*C*sin(beta)*sin(beta)*cos(alfa));
	e(3) = 2*(1+2*(cos(beta)*cos(beta)-sin(gama)*sin(gama))-2*A*(1+2*cos(beta)*cos(alfa)*cos(gama))
			+B*(1+2*cos(alfa)*cos(alfa))+2/a/a*(b*b*sin(gama)*sin(gama)+c*c*sin(beta)*sin(beta)));
	e(4) = 4*(-cos(beta)*cos(gama)+A*(cos(beta)*cos(gama)+cos(alfa))-B*cos(alfa)
			-2*D*sin(gama)*sin(gama)*cos(alfa));
	e(5) = 1-2*A+B+4*D*sin(gama)*sin(gama);
	double b23 = b, c13 = c, beta23 = beta, gama13 = gama;
#ifdef DEBUG 
	dout.setf(std::ios_base::fixed);
	dout<<'\n'<<"123 - alfa, ...: "<<alfa*200/M_PI<<' '<<beta*200/M_PI<<' '<<gama*200/M_PI<<' '<<a<<' '<<b<<' '<<c<<' '<<A<<' '<<B<<' '<<C<<' '<<D;	
#endif
	//123

	//124	
	a = distance_3d(xa,1,2);
	b = distance_3d(xa,2,4);
	c = distance_3d(xa,1,4);
	A = (b*b+c*c)/(a*a);
	B = ( (b*b-c*c)/(a*a) * (b*b-c*c)/(a*a) );
	C = c*c/(a*a);
	D = b*b/(a*a);	

	alfa = inner_angle(Xa,1,2);
	beta = inner_angle(Xa,2,4);
	gama = inner_angle(Xa,1,4);

	ef(1) = 1 - 2*A + B + 4*C*sin(beta)*sin(beta);
	ef(2) = 4*(-cos(beta)*cos(gama)+A*(cos(beta)*cos(gama)+cos(alfa))-B*cos(alfa)
			-2*C*sin(beta)*sin(beta)*cos(alfa));
	ef(3) = 2*(1+2*(cos(beta)*cos(beta)-sin(gama)*sin(gama))-2*A*(1+2*cos(beta)*cos(alfa)*cos(gama))
			+B*(1+2*cos(alfa)*cos(alfa))+2/a/a*(b*b*sin(gama)*sin(gama)+c*c*sin(beta)*sin(beta)));
	ef(4) = 4*(-cos(beta)*cos(gama)+A*(cos(beta)*cos(gama)+cos(alfa))-B*cos(alfa)
			-2*D*sin(gama)*sin(gama)*cos(alfa));
	ef(5) = 1-2*A+B+4*D*sin(gama)*sin(gama);	
	//124

	//1and2
	Vec<> e1(4),f1(4);
	for(int i = 2; i<=5; i++)	e1(i-1)=e(i)/e(1)-ef(i)/ef(1);
	for(int i = 1; i<=4; i++)	f1(i)=e(i)/e(5)-ef(i)/ef(5);
	//1and2

	//3and4
	Vec<> e2(3),f2(3);
	for(int i = 2; i<=4; i++)	e2(i-1)=e1(i)/e1(1)-f1(i)/f1(1);
	for(int i = 1; i<=3; i++)	f2(i)=e1(i)/e1(4)-f1(i)/f1(4);
	//3and4

	//5and6
	Vec<> e3(2),f3(2);
	for(int i = 2; i<=3; i++)	e3(i-1)=e2(i)/e2(1)-f2(i)/f2(1);
	for(int i = 1; i<=2; i++)	f3(i)=e2(i)/e2(3)-f2(i)/f2(3);
	
	//5and6
	double v1 = -e3(2)/e3(1);
	double v2 = -f3(2)/f3(1);
	double v = (v1+v2)/2;

	//r
	double r1a,r2,r3,r4;
	r1a = a/sqrt(1+v*v-2*v*cos(alfa));
	r2 = v*r1a;
	r3 = (r1a*r1a-r2*r2-c13*c13+b23*b23)/2/(r1a*cos(gama13)-r2*cos(beta23));
	r4 = (r1a*r1a-r2*r2-c*c+b*b)/2/(r1a*cos(gama)-r2*cos(beta));

#ifdef DEBUG
	dout<<'\n'<<"124 - alfa, ...: "<<alfa*200/M_PI<<' '<<beta*200/M_PI<<' '<<gama*200/M_PI<<' '<<a<<' '<<b<<' '<<c<<' '<<A<<' '<<B<<' '<<C<<' '<<D;

	dout<<'\n'<<'\n'<<"a1*v^4+ a2*v^3 + ...: "<<'\n';
	for(int i=1;i<=5;i++) dout<<e(i)<<' ';
	dout<<'\n';
	for(int i=1;i<=5;i++) dout<<ef(i)<<' ';

	dout<<'\n'<<'\n'<<"a1*v^3+ a2*v^2 + ...: "<<'\n';
	for(int i=1;i<=4;i++) dout<<e1(i)<<' ';
	dout<<'\n';
	for(int i=1;i<=4;i++) dout<<f1(i)<<' ';	
	
	dout<<'\n'<<'\n'<<"a1*v^2+ a2*v + ...: "<<'\n';
	for(int i=1;i<=3;i++) dout<<e2(i)<<' ';
	dout<<'\n';
	for(int i=1;i<=3;i++) dout<<f2(i)<<' ';

	dout<<'\n'<<'\n'<<"a1*v + a2 = 0: "<<'\n';
	for(int i=1;i<=2;i++) dout<<e3(i)<<' ';
	dout<<'\n';
	for(int i=1;i<=2;i++) dout<<f3(i)<<' ';

	dout<<'\n'<<'\n'<<"v1, v2, v: "<<v1<<' '<<v2<<' '<<v;

	dout<<'\n'<<'\n'<<"r1, r2, ...: "<<r1a<<' '<<r2<<' '<<r3<<' '<<r4;
	dout<<endl;
#endif

	Vec<> OP(4),scale(4);
		
	OP(1) = sqrt( (Xa(1,1)-x0)*(Xa(1,1)-x0) + (Xa(1,2)-y0)*(Xa(1,2)-y0) + f*f );
	OP(2) = sqrt( (Xa(2,1)-x0)*(Xa(2,1)-x0) + (Xa(2,2)-y0)*(Xa(2,2)-y0) + f*f ); 
	OP(3) = sqrt( (Xa(3,1)-x0)*(Xa(3,1)-x0) + (Xa(3,2)-y0)*(Xa(3,2)-y0) + f*f ); 
	OP(4) = sqrt( (Xa(4,1)-x0)*(Xa(4,1)-x0) + (Xa(4,2)-y0)*(Xa(4,2)-y0) + f*f ); 
	scale(1) = r1a/OP(1); scale(2) = r2/OP(2); scale(3) = r3/OP(3); scale(4) = r4/OP(4);

	/*
	//my new shorter solution
	//rotation
	Mat<> XXXa(4,4);
	for(int i=1;i<=4;i++)
	{
		XXXa(i,1) = scale(i)*(Xa(i,1)-x0);
		XXXa(i,2) = scale(i)*(Xa(i,2)-y0);
		XXXa(i,3) = (-1)*scale(i)*f;
		XXXa(i,4) = Xa(i,3);
	}	

	Mat<> xxxa(4,4);
	for(int i=1;i<=4;i++)
		for(int j=1;j<=4;j++)	xxxa(i,j) = xa(i,j);
	
	identity_3d i3d(xxxa,XXXa);
	i3d.solve();
#ifdef DEBUG
	i3d.report(dout);
#endif	
	Vec<> temp = i3d.get_solution();
	h(1) = temp(7);h(2) = temp(10);h(3) = temp(13);
	h(4) = temp(8);h(5) = temp(11);h(6) = temp(14);
	h(7) = temp(9);h(8) = temp(12);h(9) = temp(15);
	h(10) = temp(1);h(11) = temp(2);h(12) = temp(3);
	//my new shorter solution
	*/

	//cosinus delta
	double cdel2,cdel3;
	cdel2 = (r1a*r1a+a*a-r2*r2)/(2*r1a*a);
	cdel3 = (r1a*r1a+c13*c13-r3*r3)/(2*r1a*c13);

	Vec<> n(3);
	n(1) = (xa(2,2)-xa(1,2))*(xa(3,3)-xa(1,3))-(xa(3,2)-xa(1,2))*(xa(2,3)-xa(1,3));
	n(2) = (xa(2,3)-xa(1,3))*(xa(3,1)-xa(1,1))-(xa(2,1)-xa(1,1))*(xa(3,3)-xa(1,3));
	n(3) = (xa(2,1)-xa(1,1))*(xa(3,2)-xa(1,2))-(xa(3,1)-xa(1,1))*(xa(2,2)-xa(1,2));

	//F
	Mat<> AA(3,3);
	AA(1,1) = xa(2,1)-xa(1,1); AA(1,2) = xa(2,2)-xa(1,2); AA(1,3) = xa(2,3)-xa(1,3);
	AA(2,1) = xa(3,1)-xa(1,1); AA(2,2) = xa(3,2)-xa(1,2); AA(2,3) = xa(3,3)-xa(1,3);
	AA(3,1) = n(1); AA(3,2) = n(2); AA(3,3) = n(3);
	Vec<> AL(3);
	AL(1) = (xa(2,1)-xa(1,1))*xa(1,1) + (xa(2,2)-xa(1,2))*xa(1,2) + (xa(2,3)-xa(1,3))*xa(1,3) + a*r1a*cdel2;
	AL(2) = (xa(3,1)-xa(1,1))*xa(1,1) + (xa(3,2)-xa(1,2))*xa(1,2) + (xa(3,3)-xa(1,3))*xa(1,3) + c13*r1a*cdel3;
	AL(3) = n(1)*xa(1,1) + n(2)*xa(1,2) + n(3)*xa(1,3);

	Vec<> ff = inv(AA)*AL;
	//O
	double fo = sqrt( r1a*r1a - (ff(1)-xa(1,1))*(ff(1)-xa(1,1)) - (ff(2)-xa(1,2))*(ff(2)-xa(1,2)) - (ff(3)-xa(1,3))*(ff(3)-xa(1,3)) );
	Vec<> o = ff + 1.0/e_norm(n)*fo*n;

#ifdef DEBUG
	//o(1) = 120.064;	o(2) = 159.823;	o(3) = 88.189;
	//r1a = 123.92; r2 = 114.25; r3 = 80.28;
	dout<<'\n'<<"del: "<<acos(cdel2)*200/M_PI<<' '<<acos(cdel3)*200/M_PI;
	dout<<'\n'<<"n: "<<n;
	dout<<'\n'<<"AA: "<<AA;
	dout<<'\n'<<"AL: "<<AL;
	dout<<'\n'<<"F: "<<ff;
	dout<<'\n'<<"O: "<<o<<flush;
#endif	

	//ijk from image 
	Mat<> XXa(3,3);
	for(int i=1;i<=3;i++)
	{
		XXa(i,1) = scale(i)*(Xa(i,1)-x0);
		XXa(i,2) = scale(i)*(Xa(i,2)-y0);
		XXa(i,3) = (-1)*scale(i)*f;
	}	

	Mat<> R_img(3,3);
	Vec<> p12(3),p13(3),vi(3),vj(3),vk(3);

	for(int i=1;i<=3;i++)	
	{
		p12(i) = XXa(2,i)-XXa(1,i);
		p13(i) = XXa(3,i)-XXa(1,i);
	}
	
	vi = p12;
	double norm = e_norm(vi);
	for(int i=1;i<=3;i++)	vi(i) = vi(i)/norm;
		
	vk = cross_product(p12,p13);
	norm = e_norm(vk);
	for(int i=1;i<=3;i++)	vk(i) = vk(i)/norm;

	vj = cross_product(vk,vi);

	for(int i=1;i<=3;i++)
	{
		R_img(i,1) = vi(i);
		R_img(i,2) = vj(i);
		R_img(i,3) = vk(i);
	}
	//ijk from image
	
	//ijk from object 
	Mat<> R_obj(3,3);

	for(int i=1;i<=3;i++)	
	{
		p12(i) = xa(2,i)-xa(1,i);
		p13(i) = xa(3,i)-xa(1,i);
	}
	
	vi = p12;
	norm = e_norm(vi);
	for(int i=1;i<=3;i++)	vi(i) = vi(i)/norm;
		
	vk = cross_product(p12,p13);
	norm = e_norm(vk);
	for(int i=1;i<=3;i++)	vk(i) = vk(i)/norm;

	vj = cross_product(vk,vi);

	for(int i=1;i<=3;i++)
	{
		R_obj(i,1) = vi(i);
		R_obj(i,2) = vj(i);
		R_obj(i,3) = vk(i);
	}
	//ijk from object 
	Mat<> R = R_obj*trans(R_img);
	
	//ijk2 from image according to Krauss
	Mat<> R_img2(3,3);
	
	for(int i=1;i<=2;i++)	
	{
		p12(i) = Xa(2,i)-Xa(1,i);
		p13(i) = Xa(3,i)-Xa(1,i);
	}
	p12(3) = 0; p13(3) = 0;
	
	vi = p12;
	norm = e_norm(vi);
	for(int i=1;i<=3;i++)	vi(i) = vi(i)/norm;
		
	vk = cross_product(p12,p13);
	norm = e_norm(vk);
	for(int i=1;i<=3;i++)	vk(i) = vk(i)/norm;

	vj = cross_product(vk,vi);

	for(int i=1;i<=3;i++)
	{
		R_img2(i,1) = vi(i);
		R_img2(i,2) = vj(i);
		R_img2(i,3) = vk(i);
	}
	//ijk2 from image according to Krauss
		
	//ijk2 from object according to Krauss
	Mat<> xxa(3,3);
	for(int i=1;i<=3;i++)
	{
		xxa(i,1) = (xa(i,1)-o(1))/scale(i);
		xxa(i,2) = (xa(i,2)-o(2))/scale(i);
		xxa(i,3) = (xa(i,3)-o(3))/scale(i);
	}

	Mat<> R_obj2(3,3);

	for(int i=1;i<=3;i++)	
	{
		p12(i) = xxa(2,i)-xxa(1,i);
		p13(i) = xxa(3,i)-xxa(1,i);
	}
	
	vi = p12;
	norm = e_norm(vi);
	for(int i=1;i<=3;i++)	vi(i) = vi(i)/norm;
		
	vk = cross_product(p12,p13);
	norm = e_norm(vk);
	for(int i=1;i<=3;i++)	vk(i) = vk(i)/norm;

	vj = cross_product(vk,vi);

	for(int i=1;i<=3;i++)
	{
		R_obj2(i,1) = vi(i);
		R_obj2(i,2) = vj(i);
		R_obj2(i,3) = vk(i);
	}	
	//ijk2 from object according to Krauss
	Mat<> R2 = R_obj2*trans(R_img2);
		
	h(1) = R2(1,1);h(2) = R2(2,1);h(3) = R2(3,1);
	h(4) = R2(1,2);h(5) = R2(2,2);h(6) = R2(3,2);
	h(7) = R2(1,3);h(8) = R2(2,3);h(9) = R2(3,3);
	h(10) = o(1);h(11) = o(2);h(12) = o(3);
	
	/*
	h(1)= -0.9983404435;  
	h(2)=0.03481767372  ;
	h(3)=0.04697403727  ;
	h(4)=0.004771418125  ;
	h(5)=-0.7534175183  ;
	h(6)=0.6574833847  ;
	h(7)=0.05819663294  ;
	h(8)=0.6565105132  ;
	h(9)=0.7520368947  ;
	h(10)=386.274618  ;
	h(11)=-434.9969827  ;
	h(12)=-729.6730027 ;
	*/
	
#ifdef DEBUG
	//dout<<'\n'<<"ri "<<ri;
	dout<<'\n'<<"OP "<<OP;
	dout<<'\n'<<"scale "<<scale;
	dout<<'\n'<<"XXa: "<<XXa;
	dout<<'\n'<<"R_img: "<<R_img;
	dout<<'\n'<<"R_obj: "<<R_obj;
	dout<<'\n'<<"R: "<<R;
	dout<<'\n'<<"xxa: "<<xxa;
	dout<<'\n'<<"R_img2: "<<R_img2;
	dout<<'\n'<<"R_obj2: "<<R_obj2;
	dout<<'\n'<<"R2: "<<R2;	
	dout<<flush;
#endif		
}

void fill_matrixes_projective_inner()
{	
	//J a X0T	
	double r11=h(1),r12=h(2),r13=h(3);
	double r21=h(4),r22=h(5),r23=h(6);
	double r31=h(7),r32=h(8),r33=h(9);
	double X0=h(10),Y0=h(11),Z0=h(12);
	
	double A,B,C;
	for(int i = 1; i <= r1; i++)
	{			
		A = r11*((*xi)(i,1)-X0)+r12*((*xi)(i,2)-Y0)+r13*((*xi)(i,3)-Z0);
		B = r21*((*xi)(i,1)-X0)+r22*((*xi)(i,2)-Y0)+r23*((*xi)(i,3)-Z0);
		C = r31*((*xi)(i,1)-X0)+r32*((*xi)(i,2)-Y0)+r33*((*xi)(i,3)-Z0);		
		X0T(i,1) = h(13)-h(15)*A/C;
		X0T(i,2) = h(14)-h(15)*B/C; 
		
		J((i-1)*n+1,1) = -h(15)*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+1,2) = -h(15)*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+1,3) = -h(15)*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+1,7) = h(15)*A/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+1,8) = h(15)*A/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+1,9) = h(15)*A/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+1,10) = h(15)*(r11*C-r31*A)/C/C;
		J((i-1)*n+1,11) = h(15)*(r12*C-r32*A)/C/C; 
		J((i-1)*n+1,12) = h(15)*(r13*C-r33*A)/C/C; 
		J((i-1)*n+1,13) = 1;		
		J((i-1)*n+1,15) = -A/C; 

		J((i-1)*n+2,4) = -h(15)*((*xi)(i,1)-X0)/C; 
		J((i-1)*n+2,5) = -h(15)*((*xi)(i,2)-Y0)/C;
		J((i-1)*n+2,6) = -h(15)*((*xi)(i,3)-Z0)/C;
		J((i-1)*n+2,7) = h(15)*B/C/C*((*xi)(i,1)-X0); 
		J((i-1)*n+2,8) = h(15)*B/C/C*((*xi)(i,2)-Y0); 
		J((i-1)*n+2,9) = h(15)*B/C/C*((*xi)(i,3)-Z0); 
		J((i-1)*n+2,10) = h(15)*(r21*C-r31*B)/C/C;
		J((i-1)*n+2,11) = h(15)*(r22*C-r32*B)/C/C; 
		J((i-1)*n+2,12) = h(15)*(r23*C-r33*B)/C/C;
		J((i-1)*n+2,14) = 1;		
		J((i-1)*n+2,15) = -B/C; 
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

void fill_matrixes() {fill_matrixes_projective_inner();}

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
			X(i,1) = h(13)-h(15)*A/C;
			X(i,2) = h(14)-h(15)*B/C; 
		}	
		return X;
	}	
	else
	{
		throw at_exception("projective_inner::transform","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

double projective_inner::inner_angle(Mat<>& Xa, int a, int b)
{
	double da, db, dc;
	Mat<> A(3,3);
	A(1,1) = Xa(a,1)-x0; A(1,2) = Xa(a,2)-y0; A(1,3) = -f;
	A(2,1) = Xa(b,1)-x0; A(2,2) = Xa(b,2)-y0; A(2,3) = -f;
	A(3,1) = 0; A(3,2) = 0; A(3,3) = 0;
	da = distance_3d(A,1,3);
	db = distance_3d(A,2,3);
	dc = distance_3d(A,1,2);
	return acos( (da*da+db*db-dc*dc)/2/da/db );
}	

double compute_area(const Mat<>& chull, int i1, int i2, int i3)
{
	//one half of cross product size
	double v1x = chull(i2,1) - chull(i1,1);
	double v1y = chull(i2,2) - chull(i1,2);
	double v2x = chull(i3,1) - chull(i1,1);
	double v2y = chull(i3,2) - chull(i1,2);
	double vsx = v1y - v2y;
	double vsy = v2x - v1x;
	double vsz = v1x*v2y - v2x*v1y;
	return 0.5*sqrt(vsx*vsx+vsy*vsy+vsz*vsz);
}

void three_from_n_combination(const Mat<>& chull,Mat<>& Xa,const int comb)
{
#ifdef DEBUG
	kout<<'\n'<<"three_from_n_combination"<<endl;
#endif
	double s = 0, si;
	int pp = chull.rows();
	int s1,s2,s3,i1,i2,i3;
	srand((unsigned)time(NULL));
	for(int i=1;i <= comb;i++) 
	{		
		i1 = rand()%pp + 1;
		i2 = i1;
		while(i2 == i1)	i2 = rand()%pp + 1;
		i3 = i2;
		while(i3 == i2 || i3 == i1)	i3 = rand()%pp + 1;
		si = compute_area(chull,i1,i2,i3);
#ifdef DEBUG
		kout<<'\n'<<"combination: "<<comb<<' '<<i1<<' '<<i2<<' '<<i3<<" area: "<<si;
#endif	
		if( s < si ) 
		{
			s = si;
			s1 = i1; s2 = i2; s3 = i3;
		}
	}
	Xa(1,1) = chull(s1,1); Xa(1,2) = chull(s1,2);	Xa(1,3) = chull(s1,3);
	Xa(2,1) = chull(s2,1); Xa(2,2) = chull(s2,2);	Xa(2,3) = chull(s2,3);
	Xa(3,1) = chull(s3,1); Xa(3,2) = chull(s3,2);	Xa(3,3) = chull(s3,3);
}
	
void three_from_all_combination(const Mat<>& chull,Mat<>& Xa,const int all_comb)
{	
#ifdef DEBUG
	kout<<'\n'<<"three_from_all_combination: "<<all_comb<<endl;
#endif
	double s = 0, si;
	int pp = chull.rows();
	int s1,s2,s3;
	for(int i=1; i<=pp-2; i++)
		for(int j=i+1; j<=pp-1; j++)
			for(int k=j+1; k<=pp; k++)
			{		
				si = compute_area(chull,i,j,k);
#ifdef DEBUG
				kout<<'\n'<<"combination: "<<i<<' '<<j<<' '<<k<<" area: "<<si;
#endif
				if( s < si ) 
				{
					s = si;
					s1 = i; s2 = j; s3 = k;
				}
			}
	Xa(1,1) = chull(s1,1); Xa(1,2) = chull(s1,2);	Xa(1,3) = chull(s1,3);
	Xa(2,1) = chull(s2,1); Xa(2,2) = chull(s2,2);	Xa(2,3) = chull(s2,3);
	Xa(3,1) = chull(s3,1); Xa(3,2) = chull(s3,2);	Xa(3,3) = chull(s3,3);
}

void projective_inner::choose_four_suitable_points(Mat<>& Xa)
{
	//create convex hull
	Vec<int> ch(r1);
	int pocet = 1;
	int best = 1; //index

	//up right
	for(int i = 2; i<=r1; i++)	
		if( (*Xi)(i,2)<=(*Xi)(best,2) )	
		{
			if( (*Xi)(i,2)<(*Xi)(best,2) )	best = i;
			else if ( (*Xi)(i,1)>(*Xi)(best,1) )	best = i;
		}
	ch(1) = best;
#ifdef DEBUG
	kout<<'\n'<<"up right "<<(*Xi)(best,3)<<endl;
#endif
	//left up
	
	//bearing and left_angle
	best = 1;	
	double bearing_ii, bearing_best;
	
	//bearing on the first point - minimal		
	bearing_best = 400;
	for(int i = 1; i<=r1; i++)
	{
		if(ch(1) != i)
		{
			bearing_ii = bearing(*Xi,ch(1),i);
			if( bearing_ii <= bearing_best )
			{
				if( bearing_ii < bearing_best )	
				{
					bearing_best = bearing_ii;
					best = i;
				}
				else if ( distance_2d(*Xi,ch(1),i) < distance_2d(*Xi,ch(1),best) )	best = i; //the closest
			}
		}
	}
	ch(2) = best;
	pocet++;
	//bearing on the first point
#ifdef DEBUG
	kout<<'\n'<<"smallest bearing "<<(*Xi)(best,3)<<endl;
#endif
	bool all = false;
	double angle_ii, angle_best; // minimal angle
	while(!all)
	{		
		bearing_ii = bearing(*Xi,ch(pocet),ch(pocet-1));
		angle_best = 400;		
		for(int i = 1; i<=r1; i++)
		{
			if(ch(pocet) != i)
			{
				angle_ii = bearing(*Xi,ch(pocet),i) - bearing_ii;
				if(angle_ii <= 0)	angle_ii += 2.0*M_PI;			
				//kout<<'\n'<<"angle ii "<<angle_ii*200/M_PI<<" point "<<(*Xi)(i,3);
				if( angle_ii <= angle_best )
				{
					if( angle_ii < angle_best )	
					{
						angle_best = angle_ii;
						best = i;
					}
					else if ( distance_2d(*Xi,ch(pocet),i) < distance_2d(*Xi,ch(pocet),best) )	best = i;	//the closest
				}
			}
		}		
		if( best == ch(1) )	all = true;		
		else
		{
			pocet++;
			ch(pocet) = best;
#ifdef DEBUG
			kout<<'\n'<<"next point "<<(*Xi)(best,3)<<endl;
#endif
		}				
	}
	//bearing and left_angle	

	Mat<> chull(pocet,3);
	for (int i = 1; i<= pocet; i++)	
	{		
		chull(i,1) = (*Xi)(ch(i),1);
		chull(i,2) = (*Xi)(ch(i),2);
		chull(i,3) = (*Xi)(ch(i),3);
	}
	
	//compute number of combinations
	int all_comb = pocet*(pocet-1)*(pocet-2)/6;
	int comb = 200;
	//compute number of combinations

	//compute best three point combination
	if(all_comb > comb)	three_from_n_combination(chull,Xa,comb);
	else	three_from_all_combination(chull,Xa,all_comb);
#ifdef DEBUG
	kout<<'\n'<<"best three point combination: "<<Xa(1,3)<<' '<<Xa(2,3)<<' '<<Xa(3,3)<<endl;
#endif			
	//compute best the fourth point
	double s = 0, si, sii;		
	// p1, p2 and p3 index
	int p1,p2,p3;
	const int n1 = Xa(1,3), n2 = Xa(2,3),n3 = Xa(3,3);
	for(int i = 1; i <= r1; i++)
	{
		if( (*Xi)(i,3) == n1 )	p1 = i;
		if( (*Xi)(i,3) == n2 )	p2 = i;
		if( (*Xi)(i,3) == n3 )	p3 = i;
	}	
	// p1, p2 and p3 index
	for(int i = 1; i <= r1; i++)
	{		
		si = compute_area(*Xi,p1,p2,i);
		sii = compute_area(*Xi,p1,p3,i);
		if(si > sii)	si = sii;
		sii = compute_area(*Xi,p2,p3,i);
		if(si > sii)	si = sii;
		if(s < si) //the largest the smallest triangle area
		{
			s = si;
			best = i;
		}
#ifdef DEBUG
		kout<<'\n'<<i<<" area "<<si;
#endif
	}
	//compute the best fourth point
	Xa(4,1) = (*Xi)(best,1); Xa(4,2) = (*Xi)(best,2);	Xa(4,3) = (*Xi)(best,3);
#ifdef DEBUG
	kout<<'\n'<<Xa<<endl;
	kout<<'\n'<<"the best fourth point: "<<Xa(4,3)<<endl;
#endif
}

class projective_x0y0 : public projective_inner 
{
public:
projective_x0y0(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_inner(XXi,xxi,v)  
        {tt="projective_x0y0"; q=7;}

projective_x0y0(const Vec<> hh, const Vec<> v) : projective_inner(hh,v)  
        {tt="projective_x0y0"; q=7;}

projective_x0y0() : projective_inner()  
        {tt="projective_x0y0"; q=7;}

virtual ~projective_x0y0 (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_inner
void fill_matrixes_projective()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T		
	b(7) = - h(15) + f;		
	B(15,7) = 1;
}

void fill_matrixes() 
{
	fill_matrixes_projective_inner();
	fill_matrixes_projective();
}

//void transform_points(){}	-define in projective_inner			
};


class projective_f : public projective_inner 
{
public:
projective_f(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_inner(XXi,xxi,v)  
        {tt="projective_f"; q=8;}

projective_f(const Vec<> hh, const Vec<> v) : projective_inner(hh,v)  
        {tt="projective_f"; q=8;}

projective_f() : projective_inner()  
        {tt="projective_f"; q=8;}

virtual ~projective_f (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_inner
void fill_matrixes_projective()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T		
	b(7) = - h(13) + x0;
	b(8) = - h(14) + y0;		
	B(13,7) = 1;
	B(14,8) = 1;
}

void fill_matrixes() 
{
	fill_matrixes_projective_inner();
	fill_matrixes_projective();
}

//void transform_points(){}	-define in projective_inner			
};


class projective : public projective_inner 
{
public:
projective(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : projective_inner(XXi,xxi,v)  
        {tt="projective"; q=9;}

projective(const Vec<> hh, const Vec<> v) : projective_inner(hh,v)  
        {tt="projective"; q=9;}

projective() : projective_inner()  
        {tt="projective"; q=9;}

virtual ~projective (){}

//ostream& report_constants(ostream& out) const    -define in projective_inner  
protected:

//void ApproxSolution(){}  -define in projective_inner
void fill_matrixes_projective()
{	
	//constraints
	//dX directly instead of X0T, dX = -X0T		
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
	fill_matrixes_projective();
}

//void transform_points(){}	-define in projective_inner			
};



}   // namespace spat_fig
#endif