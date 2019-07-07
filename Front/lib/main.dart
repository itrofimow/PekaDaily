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
  String _pekaUrl;

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

  Future<String> getPekaAddr() async {
    try {
      String response;
      await Future.wait([
        http.get('http://192.168.1.3:1337/api/current').then((res) => response = res.body), 
        Future.delayed(Duration(seconds: 2))]);
      return response;
    }
    catch (e) {
      return '_failed';
    }
  }

  @override
  void initState() {
    super.initState();

    _firebaseMessaging.configure(
      onMessage: (Map<String, dynamic> message) async {
        print("onMessage: $message");
      },
      onLaunch: (Map<String, dynamic> message) async {
        print("onLaunch: $message");
      },
      onResume: (Map<String, dynamic> message) async {
        print("onResume: $message");
      },
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

    getPekaAddr().then((res) {
      if (mounted) {
        setState(() {
          _pekaUrl = res;
        });
      }
    });
  }

  Widget _buildPeka() {
    return Container(
      height: 200,
      width: 200,
      color: Colors.transparent,
      child: _pekaUrl == null
        ? Container()
        : _pekaUrl == '_failed'
          ? Image.asset('assets/1.png', 
            width: 200, 
            height: 200)
          : Image.network(_pekaUrl,
            width: 200, 
            height: 200,)
    );
  }

  Widget _buildText() {
    return Container(
      margin: EdgeInsets.only(top: 25),
      child: _pekaUrl == null
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
