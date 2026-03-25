import 'package:json_annotation/json_annotation.dart';

part 'question_response.g.dart';

@JsonSerializable()
class QuestionResponse {
  final int id;
  final int userId;
  final String? userName;

  final String title;
  final String content;
  final String status;

  final String? answer;

  final DateTime createdAt;
  final DateTime? answeredAt;

  QuestionResponse({
    required this.id,
    required this.userId,
    this.userName,
    required this.title,
    required this.content,
    required this.status,
    this.answer,
    required this.createdAt,
    this.answeredAt,
  });

  factory QuestionResponse.fromJson(Map<String, dynamic> json) =>
      _$QuestionResponseFromJson(json);

  Map<String, dynamic> toJson() => _$QuestionResponseToJson(this);
}