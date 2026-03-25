import 'package:json_annotation/json_annotation.dart';

part 'answer_response.g.dart';

@JsonSerializable()
class AnswerResponse {
  final int id;
  final int questionId;
  final String? questionTitle;
  final int adminId;
  final String? adminUserName;
  final String message;
  final DateTime createdAt;

  AnswerResponse({
    required this.id,
    required this.questionId,
    this.questionTitle,
    required this.adminId,
    this.adminUserName,
    required this.message,
    required this.createdAt,
  });

  factory AnswerResponse.fromJson(Map<String, dynamic> json) =>
      _$AnswerResponseFromJson(json);

  Map<String, dynamic> toJson() => _$AnswerResponseToJson(this);
}