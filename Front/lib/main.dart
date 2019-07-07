import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/services.dart';

void main() {;
  SystemChrome.setSystemUIOverlayStyle(SystemUiOverlayStyle(
    systemNavigationBarColor: Color.fromRGBO(255, 191, 0, 1)
  ));

  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'PekaDaily',
      theme: ThemeData(
        // This is the theme of your application.
        //
        // Try running your application with "flutter run". You'll see the
        // application has a blue toolbar. Then, without quitting the app, try
        // changing the primarySwatch below to Colors.green and then invoke
        // "hot reload" (press "r" in the console where you ran "flutter run",
        // or simply save your changes to "hot reload" in a Flutter IDE).
        // Notice that the counter didn't reset back to zero; the application
        // is not restarted.
        primarySwatch: Colors.orange,
      ),
      home: MyHomePage(),
    );
  }
}

class MyHomePage extends StatefulWidget {
  MyHomePage({Key key}) : super(key: key);

  @override
  _MyHomePageState createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  final _firebaseMessaging = FirebaseMessaging();
  
  bool loaded = false;
  bool failed = false;
  Image image;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            colors: [
              Color.fromRGBO(255, 221, 45, 1),
              Color.fromRGBO(255, 191, 0, 1),
            ],
            begin: Alignment.topLeft,
            end: Alignment.bottomRight
          ),
        ),
        child: Center(child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _buildPeka(),
            _buildText()
          ])
        ),
      )
    );
  }

  Future loadPeka() async {
    try {
      String response;
      await Future.wait([
        http.get('http://achiever.gg/api/current').then((res) {
          response = 'http://achiever.gg/img/' + res.body;
        }), 
        Future.delayed(Duration(seconds: 1))]);

      image = Image.network(response);
      image.image.resolve(ImageConfiguration()).addListener((i, b) {
        if (mounted) {
          setState(() {
            loaded = true;
          });
        }
      });
    }
    catch (e) {
      if (mounted) {
        setState(() {
          failed = true;
        });
      }
    }
  }

  Future resetPeka() async {
    if (mounted) {
      setState(() {
        loaded = false;
        failed = false;
      });
    }
    
    await loadPeka();
  }

  @override
  void initState() {
    super.initState();

    _firebaseMessaging.configure(
      onMessage: (Map<String, dynamic> message) => resetPeka(),
      onLaunch: (Map<String, dynamic> message) async {
        print("onLaunch: $message");
      },
      onResume: (Map<String, dynamic> message) {
        return resetPeka();
      }
    );
    _firebaseMessaging.requestNotificationPermissions(
        const IosNotificationSettings(sound: true, badge: true, alert: true));
    _firebaseMessaging.onIosSettingsRegistered
        .listen((IosNotificationSettings settings) {
      print("Settings registered: $settings");
    });

    _firebaseMessaging.subscribeToTopic('all');

    _firebaseMessaging.getToken().then((val) {
      print(val);
    });
    
    resetPeka();
  }

  Widget _buildPeka() {
    Widget _image = Container();
    
    if (failed) _image = Image.asset('assets/1.png', 
      width: 200, 
      height: 200);

    if (!failed && loaded) _image = image;

    return Container(
      height: 200,
      width: 200,
      color: Colors.transparent,
      child: image
    );
  }

  Widget _buildText() {
    return Container(
      margin: EdgeInsets.only(top: 25),
      child: !loaded
        ? Text('peka incoming...')
        : Text('Today\'s Peka', style: const TextStyle(
          fontWeight: FontWeight.w700,
          fontSize: 34,
          letterSpacing: 0.41,
          color: Color.fromRGBO(51, 51, 51, 1)
        ))
    );
  }
}
